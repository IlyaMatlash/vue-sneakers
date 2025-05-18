import { ref } from 'vue'
import axios from 'axios'

export function useFavorites() {
  const favorites = ref([])
  const isLoading = ref(false)
  const error = ref(null)
  const sessionId = ref(localStorage.getItem('sessionId') || generateSessionId())
  function generateSessionId() {
    const newSessionId = Math.random().toString(36).substring(2, 15)
    localStorage.setItem('sessionId', newSessionId)
    return newSessionId
  }
  // Загрузка избранных товаров с сервера
  const fetchFavorites = async () => {
    try {
      isLoading.value = true
      error.value = null
      const { data } = await axios.get(`http://localhost:5072/api/favorite?sessionId=${sessionId.value}`)
      favorites.value = data
      
      // Синхронизация с localStorage
      data.forEach(favorite => {
        localStorage.setItem(`favorite_${favorite.ProductId}`, 'true')
      })
    } catch (err) {
      console.error('Error fetching favorites:', err)
      error.value = 'Не удалось загрузить избранные товары'
    } finally {
      isLoading.value = false
    }
  }

  // Добавление товара в избранное
  const addToFavorite = async (product) => {
    try {
      isLoading.value = true
      error.value = null
      const response = await axios.post('http://localhost:5072/api/favorite', {
        SessionId: sessionId.value,
        ProductId: product.ProductId
      })

      if (response.data.id) {
        const favoriteItem = {
          FavoriteId: response.data.id,
          ProductId: product.ProductId,
          SessionId: sessionId.value
        }
        favorites.value.push(favoriteItem)
        localStorage.setItem(`favorite_${product.ProductId}`, 'true')
        
        // Создаем и отправляем событие для обновления UI
        window.dispatchEvent(new CustomEvent('favorite-added', { 
          detail: { productId: product.ProductId, favoriteId: response.data.id } 
        }))
      }
      return response.data
    } catch (err) {
      console.error('Error adding to favorites:', err)
      error.value = 'Не удалось добавить товар в избранное'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Удаление товара из избранного
  const removeFromFavorite = async (favoriteId, productId) => {
    try {
      isLoading.value = true
      error.value = null
      await axios.delete(`http://localhost:5072/api/favorite/${favoriteId}`)
      favorites.value = favorites.value.filter(f => f.FavoriteId !== favoriteId)
      localStorage.removeItem(`favorite_${productId}`)
      
      // Создаем и отправляем событие для обновления UI
      window.dispatchEvent(new CustomEvent('favorite-removed', { 
        detail: { productId, favoriteId } 
      }))
      
      return true
    } catch (err) {
      console.error('Error removing from favorites:', err)
      error.value = 'Не удалось удалить товар из избранного'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Проверка, находится ли товар в избранном
  const isFavorite = (productId) => {
    return favorites.value.some(f => f.ProductId === productId) || 
           localStorage.getItem(`favorite_${productId}`) === 'true'
  }

  // Получение FavoriteId по ProductId
  const getFavoriteId = (productId) => {
    const favorite = favorites.value.find(f => f.ProductId === productId)
    return favorite ? favorite.FavoriteId : null
  }

  // Переключение статуса избранного
  const toggleFavorite = async (product) => {
    const productIsFavorite = isFavorite(product.ProductId)
    const favoriteId = getFavoriteId(product.ProductId)
    
    if (productIsFavorite && favoriteId) {
      await removeFromFavorite(favoriteId, product.ProductId)
      return false
    } else if (!productIsFavorite) {
      await addToFavorite(product)
      return true
    }
    return productIsFavorite
  }

  // Загружаем избранные при инициализации
  fetchFavorites()

  return {
    favorites,
    isLoading,
    error,
    fetchFavorites,
    addToFavorite,
    removeFromFavorite,
    isFavorite,
    getFavoriteId,
    toggleFavorite
  }
}
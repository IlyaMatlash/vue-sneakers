<script setup>
import { ref, onMounted, computed } from 'vue'
import { inject } from 'vue'
import axios from 'axios'
import CardList from '../components/CardList.vue'
import InfoBlock from '@/components/InfoBlock.vue';
import CardModal from '@/components/CardModal.vue';

const { cart, addToCart, removeFromCart } = inject('cart')
const { favorites, toggleFavorite, fetchFavorites } = inject('favorites')

const favoriteProducts = ref([])
const modalIsOpen = ref(false)
const selectedItem = ref(null)
const isLoading = ref(false)

const onClickAddToCart = (item) => {
  if (!item.isAdded) {
    addToCart(item);
    item.isAdded = true;
  } else {
    removeFromCart(item);
    item.isAdded = false;
  }
}

const favoriteItems = computed(() => {
  return favoriteProducts.value.map(product => ({
    ...product,
    isFavorite: true,
    isAdded: cart.value.some(item => item.ProductId === product.ProductId)
  }))
})

const loadFavoriteProducts = async () => {
  try {
    isLoading.value = true
    
    // Получаем все товары
    const { data: products } = await axios.get('http://localhost:5072/api/product')
    
    if (!favorites.value || favorites.value.length === 0) {
      favoriteProducts.value = [];
      return;
    }
    // Фильтруем только избранные товары
    favoriteProducts.value = products.filter(product => 
      favorites.value.some(fav => fav.ProductId === product.ProductId)
    ).map(product => {
      const favorite = favorites.value.find(fav => fav.ProductId === product.ProductId)
      return {
        ...product,
        FavoriteId: favorite ? favorite.FavoriteId : null
      }
    })
  } catch (error) {
    console.error('Error loading favorite products:', error);
    favoriteProducts.value = [];
  } finally {
    isLoading.value = false
  }
}

const handleToggleFavorite = async (item) => {
  await toggleFavorite(item)
  // После удаления из избранного обновляем список
  await loadFavoriteProducts()
}

const openCardModal = (item) => {
  selectedItem.value = item
  modalIsOpen.value = true
}

const closeCardModal = () => {
  modalIsOpen.value = false
  selectedItem.value = null
}

onMounted(async () => {
  // Загружаем избранные товары
  await fetchFavorites()
  await loadFavoriteProducts()
  
  // Слушаем события изменения избранных товаров
  window.addEventListener('favorite-removed', async () => {
    await loadFavoriteProducts()
  })
})
</script>

<template>
  <h2 class="text-3xl font-bold font-signate">Мои Избранные</h2>

  <div>
    <InfoBlock
      v-if="favoriteItems.length === 0 && !isLoading"
      Title="Избранных нет :("
      Description="Вы ничего не добавляли в избранные"
      Image="/emoji-1.png"
    />
    <div v-else>
      <CardList 
        :items="favoriteItems"
        @add-to-favorite="handleToggleFavorite"
        @add-to-cart="onClickAddToCart"
        @click="openCardModal"
      />
    </div>
    <CardModal
    v-if="modalIsOpen"
      :item="selectedItem"
      @close="closeCardModal"
      @add-to-favorite="handleToggleFavorite"
      @add-to-cart="onClickAddToCart"
    />
  </div>
</template>

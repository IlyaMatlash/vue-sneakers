<script setup>
import { ref, onMounted, computed } from 'vue'
import { inject } from 'vue'
import axios from 'axios'
import CardList from '../components/CardList.vue'
import InfoBlock from '@/components/InfoBlock.vue';
import CardModal from '@/components/CardModal.vue';

const favorites = ref([])
const modalIsOpen = ref(false)
const selectedItem = ref(null)

const { cart, addToCart, removeFromCart } = inject('cart')
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
  return favorites.value.map(favorite => ({
    ...favorite,
    isFavorite: true,
    isAdded: cart.value.some(item => item.ProductId === favorite.ProductId)
  }))
})

const fetchFavorites = async () => {
  try {
    const [favoritesResponse, productsResponse] = await Promise.all([
      axios.get('http://localhost:5072/api/favorite'),
      axios.get('http://localhost:5072/api/product')
    ]);
    
    const favoriteIds = new Set(favoritesResponse.data.map(fav => fav.ProductId));
    const allProducts = productsResponse.data;
    
    favorites.value = allProducts.filter(product => favoriteIds.has(product.ProductId))
      .map(product => ({
        ...product,
        FavoriteId: favoritesResponse.data.find(fav => fav.ProductId === product.ProductId).FavoriteId
      }));
  } catch (error) {
    console.log(error)
  }
}

const toggleFavorite = async (item) => {
  try {
    const isFavorite = favorites.value.some(fav => fav.ProductId === item.ProductId);
    if (isFavorite) {
      // Remove from favorites
      const favoriteItem = favorites.value.find(fav => fav.ProductId === item.ProductId);
      await axios.delete(`http://localhost:5072/api/favorite/${favoriteItem.FavoriteId}`)
      favorites.value = favorites.value.filter(fav => fav.ProductId !== item.ProductId)
      item.isFavorite = false
      item.FavoriteId = null
      localStorage.removeItem(`favorite_${item.ProductId}`)
    } else {
      // Add to favorites
      const response = await axios.post('http://localhost:5072/api/favorite', {
        UserId: 1, // Здесь должен быть актуальный ID пользователя
        ProductId: item.ProductId
      });
      
      if (response.data.FavoriteId) {
        favorites.value.push({ ...item, FavoriteId: response.data.FavoriteId })
      }
    }
  } catch (error) {
    console.error('Ошибка:', error.response?.data?.error || error.message);
  }
}

onMounted(fetchFavorites)
</script>

<template>
  <h2 class="text-3xl font-bold font-signate">Мои Избранные</h2>

  <div>
    <InfoBlock
      v-if="favoriteItems.length === 0"
      Title="Избранных нет :("
      Description="Вы ничего не добавляли в избранные"
      Image="/emoji-1.png"
    />
    <div v-else>
      <CardList 
        :items="favoriteItems"
        @add-to-favorite="toggleFavorite"
        @add-to-cart="onClickAddToCart"
        @click="openCardModal"
      />
    </div>
    <CardModal
      v-if="modalIsOpen"
      :item="selectedItem"
      @close="closeCardModal"
    />
  </div>
</template>

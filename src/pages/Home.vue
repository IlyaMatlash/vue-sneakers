<script setup>
import { reactive, watch, ref, onMounted } from 'vue'
import CardList from '../components/CardList.vue'
import FilterSidebar from '../components/FilterSidebar.vue'
import axios from 'axios'
import { inject } from 'vue'

const { cart, addToCart, removeFromCart } = inject('cart')

const items = ref([])

const filters = reactive({
  sortBy: 'Name',
  searchQuery: '',
  priceRange: { min: '', max: '' },
  brands: [],
  sizes: []
})

const onChangeSelect = (event) => {
  filters.sortBy = event.target.value
}

const onChangeSearchInput = (event) => {
  filters.searchQuery = event.target.value
}

const addToFavorite = async (item) => {
  try {
    if (!item.isFavorite) {
      const response = await axios.post('http://localhost:5072/api/favorite', {
        UserId: 1,
        ProductId: item.ProductId
      })
      
      if (response.data.id) {
        item.isFavorite = true
        item.FavoriteId = response.data.id
        localStorage.setItem(`favorite_${item.ProductId}`, 'true')
      }
    } else {
      await axios.delete(`http://localhost:5072/api/favorite/${item.FavoriteId}`)
      item.isFavorite = false
      item.FavoriteId = null
      localStorage.removeItem(`favorite_${item.ProductId}`)
    }
  } catch (err) {
    console.error('Error toggling favorite:', err)
  }
}

const fetchFavorites = async () => {
  try {
    const { data } = await axios.get('http://localhost:5072/api/favorite')
    
    items.value = items.value.map((item) => {
      const favorite = data.find(f => f.ProductId === item.ProductId)
      const isFavorite = favorite || localStorage.getItem(`favorite_${item.ProductId}`) === 'true'
      
      return {
        ...item,
        isFavorite: isFavorite,
        FavoriteId: favorite ? favorite.FavoriteId : null
      }
    })
  } catch (error) {
    console.error('Error fetching favorites:', error)
  }
}

const fetchItems = async () => {
  try {
    const params = {
      sortBy: filters.sortBy
    }

    if (filters.searchQuery) {
      params.name = `*${filters.searchQuery}*`
    }

    const { data } = await axios.get(`http://localhost:5072/api/product`, {
      params
    })
    items.value = data.map((obj) => ({
      ...obj,
      isFavorite: false,
      favoriteId: null,
      isAdded: false
    }))
  } catch (error) {
    console.log(error)
  }
}

const openCardModal = () => {
  modalIsOpen.value = true
}

const onClickAddPlus = (item) => {
  if (!item.isAdded) {
    addToCart(item)
    item.isAdded = true
  } else {
    removeFromCart(item)
    item.isAdded = false
  }
}

onMounted(async () => {
  const localCart = localStorage.getItem('cartItems')
  cart.value = localCart ? JSON.parse(localCart) : []

  await fetchItems()
  await fetchFavorites()

  items.value = items.value.map((item) => ({
    ...item,
    isAdded: cart.value.some(cartItem => cartItem.ProductId === item.ProductId)
  }))
  window.addEventListener('cart-item-removed', (event) => {
    const productId = event.detail.productId;
    const item = items.value.find(i => i.ProductId === productId);
    if (item) {
      item.isAdded = false;
    }
  });
})


watch(cart, (newCart) => {
  items.value = items.value.map((item) => ({
    ...item,
    isAdded: newCart.some(cartItem => cartItem.ProductId === item.ProductId)
  }))
})


watch(filters, fetchItems)
</script>

vue
<template>
  <div class="flex gap-8">
    <!-- Основной контент -->
    <div class="flex-1">
      <div class="flex justify-between items-center">
        <h2 class="text-3xl font-bold font-signate">Все кроссовки</h2>
      </div>
      <div class="mt-10">
        <CardList :items="items" @open-card-modal="openCardModal" @add-to-favorite="addToFavorite" @add-to-cart="onClickAddPlus" />
      </div>
    </div>
    <!-- Боковой фильтр -->
    <FilterSidebar
      v-model:filters="filters"
      class="sticky top-4"
    />
  </div>
</template>

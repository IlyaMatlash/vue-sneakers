<script setup>
import { reactive, watch, ref, onMounted } from 'vue'
import CardList from '../components/CardList.vue'
import CardModal from '../components/CardModal.vue';
import axios from 'axios'
import { inject } from 'vue'

const { cart, addToCart, removeFromCart } = inject('cart')

const items = ref([])

const filters = reactive({
  sortBy: 'Name',
  searchQuery: ''
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

<template>
  <div class="flex justify-between items-center">
    <h2 class="text-3xl font-bold font-signate">Все кроссовки</h2>

    <div class="flex gap-4">
      <select
        @change="onChangeSelect"
        class="py-2 px-3 border border-slate-300 rounded-md outline-none"
        name=""
        id=""
      >
        <option value="Name">По Названию</option>
        <option value="Price">Цена по убыванию</option>
        <option value="-Price">Цена по возрастанию</option>
      </select>

      <div class="relative">
        <img class="absolute left-4 top-3" src="/search.svg" alt="" />
        <input
          @input="onChangeSearchInput"
          class="border rounded-md py-2 pl-10 pr-4 outline-none border-slate-300 focus:border-gray-400"
          type="text"
          placeholder="Поиск..."
        />
      </div>
    </div>
  </div>
  <div class="mt-10">
    <CardList :items="items" @open-card-modal="openCardModal" @add-to-favorite="addToFavorite" @add-to-cart="onClickAddPlus" />
  </div>
</template>

<script setup>
import { reactive, watch, ref, onMounted } from 'vue'
import CardList from '../components/CardList.vue'
import FilterSidebar from '../components/FilterSidebar.vue'
import axios from 'axios'
import { inject } from 'vue'

const { cart, addToCart, removeFromCart } = inject('cart')
const { toggleFavorite, isFavorite, getFavoriteId } = inject('favorites')

const items = ref([])
const isLoading = ref(false)

const filters = reactive({
  sortBy: 'Name',
  searchQuery: '',
  priceRange: { min: '', max: '' },
  brands: [],
  sizes: [],
  categories: [],
  seasons: [],
  colors: [],
  materials: [],
  features: []
})

const onChangeSelect = (event) => {
  filters.sortBy = event.target.value
}

const onChangeSearchInput = (event) => {
  filters.searchQuery = event.target.value
}

const addToFavorite = async (item) => {
  try {
    const newFavoriteStatus = await toggleFavorite(item)
    item.isFavorite = newFavoriteStatus
    item.FavoriteId = newFavoriteStatus ? getFavoriteId(item.ProductId) : null
  } catch (err) {
    console.error('Error toggling favorite:', err)
  }
}

// const fetchFavorites = async () => {
//   try {
//     const { data } = await axios.get('http://localhost:5072/api/favorite')

//     items.value = items.value.map((item) => {
//       const favorite = data.find((f) => f.ProductId === item.ProductId)
//       const isFavorite = favorite || localStorage.getItem(`favorite_${item.ProductId}`) === 'true'

//       return {
//         ...item,
//         isFavorite: isFavorite,
//         FavoriteId: favorite ? favorite.FavoriteId : null
//       }
//     })
//   } catch (error) {
//     console.error('Error fetching favorites:', error)
//   }
// }

const fetchItems = async () => {
  try {
    isLoading.value = true
    const params = new URLSearchParams()
    
    if (filters.priceRange.min) params.append('priceMin', filters.priceRange.min)
    if (filters.priceRange.max) params.append('priceMax', filters.priceRange.max)
    if (filters.brands.length) params.append('brands', filters.brands.join(','))
    if (filters.sizes.length) params.append('sizes', filters.sizes.join(','))
    if (filters.categories.length) params.append('categories', filters.categories.join(','))
    if (filters.seasons.length) params.append('seasons', filters.seasons.join(','))
    if (filters.colors.length) params.append('colors', filters.colors.join(','))
    if (filters.materials.length) params.append('materials', filters.materials.join(','))
    if (filters.features.length) params.append('features', filters.features.join(','))
    if (filters.sortBy) params.append('sortBy', filters.sortBy)
    if (filters.searchQuery) params.append('searchQuery', filters.searchQuery)

    const { data } = await axios.get(`http://localhost:5072/api/product?${params.toString()}`)
    items.value = data.map(item => ({
      ...item,
      isFavorite: isFavorite(item.ProductId),
      FavoriteId: getFavoriteId(item.ProductId),
      isAdded: cart.value.some((cartItem) => cartItem.ProductId === item.ProductId)
    }))
  } catch (error) {
    console.error('Error fetching items:', error)
  } finally {
    isLoading.value = false
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

// Обработчик обновления фильтров
const handleFiltersUpdate = (updatedFilters) => {
  Object.assign(filters, updatedFilters)
  fetchItems()
}

onMounted(async () => {
  const localCart = localStorage.getItem('cartItems')
  cart.value = localCart ? JSON.parse(localCart) : []

  await fetchItems()
  // Слушаем события изменения избранных товаров
  window.addEventListener('favorite-added', (event) => {
    const { productId, favoriteId } = event.detail
    const item = items.value.find(i => i.ProductId === productId)
    if (item) {
      item.isFavorite = true
      item.FavoriteId = favoriteId
    }
  })

  window.addEventListener('favorite-removed', (event) => {
    const { productId } = event.detail
    const item = items.value.find(i => i.ProductId === productId)
    if (item) {
      item.isFavorite = false
      item.FavoriteId = null
    }
  })

  window.addEventListener('cart-item-removed', (event) => {
    const productId = event.detail.productId
    const item = items.value.find((i) => i.ProductId === productId)
    if (item) {
      item.isAdded = false
    }
  })
})

watch(cart, (newCart) => {
  items.value = items.value.map((item) => ({
    ...item,
    isAdded: newCart.some((cartItem) => cartItem.ProductId === item.ProductId)
  }))
})

// Следим только за изменениями sortBy и searchQuery, так как
// остальные фильтры будут обновляться по кнопке "Показать"
watch(
  [() => filters.sortBy, () => filters.searchQuery],
  fetchItems,
  { deep: true }
)
</script>

<template>
  <div class="flex gap-8">
    <!-- Основной контент -->
    <div class="flex-1">
      <div class="flex justify-left items-center gap-4">
        <h2 class="text-3xl font-bold font-signate">Все кроссовки</h2>
        <!-- <div class="flex gap-4">
          <select
            @change="onChangeSelect"
            class="py-2 px-3 border border-slate-300 rounded-md outline-none"
            name="sortSelect"
            id="sortSelect"
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
        </div> -->
      </div>
      <div class="mt-10">
        <CardList
          :items="items"
          @open-card-modal="openCardModal"
          @add-to-favorite="addToFavorite"
          @add-to-cart="onClickAddPlus"
        />
        <div v-if="!isLoading && items.length === 0" class="text-center py-10 text-gray-500">
          Товары не найдены
        </div>
      </div>
    </div>
    <!-- Боковой фильтр -->
    <FilterSidebar 
    v-model:filters="filters" 
    @update:filters="handleFiltersUpdate"
    class="sticky top-4" />
  </div>
</template>

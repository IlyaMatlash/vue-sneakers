<script setup>
import { reactive, watch, ref, onMounted, computed } from 'vue'
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

// Добавляем состояние для мобильных фильтров
const isFilterOpen = ref(false)

// Вычисляем количество акти��ных фильтров
const activeFiltersCount = computed(() => {
  let count = 0
  if (filters.priceRange.min || filters.priceRange.max) count++
  if (filters.brands.length) count++
  if (filters.sizes.length) count++
  if (filters.categories.length) count++
  if (filters.seasons.length) count++
  if (filters.colors.length) count++
  if (filters.materials.length) count++
  if (filters.features.length) count++
  return count
})

// Закрываем фильтры при изменении размера экрана на десктоп
onMounted(() => {
  const handleResize = () => {
    if (window.innerWidth >= 1024) {
      isFilterOpen.value = false
    }
  }
  
  window.addEventListener('resize', handleResize)
  return () => window.removeEventListener('resize', handleResize)
})

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
  <div class="container mx-auto px-4 md:px-6 lg:px-8">
    <div class="relative mb-12">
      <div class="w-screen relative left-1/2 right-1/2 -ml-[50vw] -mr-[50vw]">
        <div class="sm:px-6 lg:px-8">
          <img 
            src="/banner_main.png" 
            alt="Shoe Care Banner"
            class="w-full h-auto object-cover rounded-lg shadow-lg cursor-pointer hover:opacity-90 transition-opacity
                    aspect-[16/9] sm:aspect-[20/9] md:aspect-[24/9] lg:aspect-[28/9] xl:aspect-[32/9]"
          />
        </div>
      </div>
    </div>
    <h1 class="text-3xl md:text-3xl font-bold font-signate mb-6 lg:hidden">
      Все кроссовки
    </h1>
    <!-- Мобильная кнопка фильтров -->
    <div class="lg:hidden flex justify-between items-center mb-4">
      <button 
        @click="isFilterOpen = !isFilterOpen"
        class="flex items-center gap-2 bg-white px-4 py-2 rounded-lg shadow-sm border border-gray-200"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-gray-500" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M3 3a1 1 0 011-1h12a1 1 0 011 1v3a1 1 0 01-.293.707L12 11.414V15a1 1 0 01-.293.707l-2 2A1 1 0 018 17v-5.586L3.293 6.707A1 1 0 013 6V3z" clip-rule="evenodd" />
        </svg>
        <span>Фильтры</span>
        <!-- Бейдж с количеством активных фильтров -->
        <span v-if="activeFiltersCount" class="ml-1 bg-sky-500 text-white text-xs px-2 py-0.5 rounded-full">
          {{ activeFiltersCount }}
        </span>
      </button>

      <!-- Сортировка -->
      <!-- <select
        @change="onChangeSelect"
        class="py-2 px-3 border border-gray-200 rounded-lg outline-none bg-white"
        name="sortSelect"
        id="sortSelect"
      >
        <option value="Name">По названию</option>
        <option value="Price">Цена по убыванию</option>
        <option value="-Price">Цена по возрастанию</option>
      </select> -->
    </div>

    <div class="flex flex-col lg:flex-row gap-6 relative">
      <!-- Оверлей для мобильных фильтров -->
      <div 
        v-if="isFilterOpen" 
        class="fixed inset-0 bg-black bg-opacity-50 z-40 lg:hidden"
        @click="isFilterOpen = false"
      ></div>
      

      <!-- Фильтры -->
      <aside 
        class="fixed lg:sticky lg:top-4 lg:h-[calc(100vh-2rem)] lg:flex-shrink-0 w-[280px] bg-white rounded-lg shadow-sm border border-gray-200 overflow-auto transform transition-transform duration-300 ease-in-out z-50 lg:z-auto lg:transform-none"
        :class="{
          'translate-x-0': isFilterOpen,
          '-translate-x-full': !isFilterOpen,
          'h-screen lg:h-auto fixed top-0 left-0': true
        }"
      >
        <FilterSidebar 
          v-model:filters="filters" 
          @update:filters="handleFiltersUpdate"
          @close="isFilterOpen = false"
        />
      </aside>

      <!-- Основной контент -->
      <main class="flex-1">
        <!-- Заголовок и поиск (для десктопа) -->
        <!-- <div class="hidden lg:flex justify-between items-center mb-6">
          <h1 class="text-3xl font-bold font-signate">Все кроссовки</h1>
          
          <div class="flex items-center gap-4">
            <select
              @change="onChangeSelect"
              class="py-2 px-3 border border-gray-200 rounded-lg outline-none bg-white"
              name="sortSelect"
              id="sortSelect"
            >
              <option value="Name">По названию</option>
              <option value="Price">Цена по убыванию</option>
              <option value="-Price">Цена по возрастанию</option>
            </select>

            <div class="relative">
              <svg xmlns="http://www.w3.org/2000/svg" class="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
              <input
                @input="onChangeSearchInput"
                class="pl-10 pr-4 py-2 border border-gray-200 rounded-lg outline-none focus:border-sky-500 transition-colors w-64"
                type="text"
                placeholder="Поиск..."
              />
            </div>
          </div>
        </div> -->

        

        <!-- Мобильный поиск -->
        <!-- <div class="relative mb-6 lg:hidden">
          <svg xmlns="http://www.w3.org/2000/svg" class="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input
            @input="onChangeSearchInput"
            class="w-full pl-10 pr-4 py-2 border border-gray-200 rounded-lg outline-none focus:border-sky-500 transition-colors"
            type="text"
            placeholder="Поиск..."
          />
        </div> -->

        <!-- Список товаров -->
        <div class="min-h-[200px]">
          <CardList
            :items="items"
            @open-card-modal="openCardModal"
            @add-to-favorite="addToFavorite"
            @add-to-cart="onClickAddPlus"
          />
          <div 
            v-if="!isLoading && items.length === 0" 
            class="text-center py-10 text-gray-500"
          >
            Товары не найдены
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

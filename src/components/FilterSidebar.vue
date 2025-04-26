vue
<script setup>
import { ref } from 'vue'
const props = defineProps({
  filters: {
    type: Object,
    required: true
  }
})
const emit = defineEmits(['update:filters'])
const priceRange = ref({
  min: '',
  max: ''
})
const brands = ref([
  'Nike',
  'Adidas',
  'New Balance',
  'Puma',
  'Reebok'
])
const sizes = ref([
  '36',
  '37',
  '38',
  '39',
  '40',
  '41',
  '42',
  '43',
  '44',
  '45'
])
const categories = ref([
  'Кроссовки',
  'Кеды',
  'Ботинки',
  'Сандалии'
])
const seasons = ref([
  'Весна',
  'Лето',
  'Осень',
  'Зима'
])
const colors = ref([
  'Черный',
  'Белый',
  'Красный',
  'Синий',
  'Зеленый'
])
const materials = ref([
  'Кожа',
  'Текстиль',
  'Замша',
  'Синтетика'
])
const features = ref([
  'Водонепроницаемые',
  'Дышащие',
  'Ортопедические',
  'Легкие'
])
const selectedBrands = ref([])
const selectedSizes = ref([])
const selectedCategories = ref([])
const selectedSeasons = ref([])
const selectedColors = ref([])
const selectedMaterials = ref([])
const selectedFeatures = ref([])
const updateFilters = () => {
  emit('update:filters', {
    ...props.filters,
    priceRange: priceRange.value,
    brands: selectedBrands.value,
    sizes: selectedSizes.value,
    categories: selectedCategories.value,
    seasons: selectedSeasons.value,
    colors: selectedColors.value,
    materials: selectedMaterials.value,
    features: selectedFeatures.value
  })
}
const clearFilters = () => {
  priceRange.value = { min: '', max: '' }
  selectedBrands.value = []
  selectedSizes.value = []
  selectedCategories.value = []
  selectedSeasons.value = []
  selectedColors.value = []
  selectedMaterials.value = []
  selectedFeatures.value = []
  updateFilters()
}
</script>
<template>
    <div class="filter-sidebar w-72 bg-white p-6 rounded-lg shadow-lg">
      <div class="flex justify-between items-center mb-6">
        <h3 class="text-xl font-bold">Фильтр</h3>
        <button @click="clearFilters" class="text-sm text-gray-500 hover:text-gray-700">
          Очистить
        </button>
      </div>
      
      <!-- Цена -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Цена</h4>
        <div class="flex gap-2">
          <input 
            type="number" 
            v-model="priceRange.min"
            placeholder="От"
            class="w-1/2 p-2 border rounded"
            @change="updateFilters"
          >
          <input 
            type="number" 
            v-model="priceRange.max"
            placeholder="До"
            class="w-1/2 p-2 border rounded"
            @change="updateFilters"
          >
        </div>
      </div>
      <!-- Категории -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Категории</h4>
        <div class="flex flex-col gap-2">
          <label v-for="category in categories" :key="category" class="flex items-center">
            <input 
              type="checkbox"
              v-model="selectedCategories"
              :value="category"
              @change="updateFilters"
              class="mr-2"
            >
            {{ category }}
          </label>
        </div>
      </div>
      <!-- Сезон -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Сезон</h4>
        <div class="flex flex-col gap-2">
          <label v-for="season in seasons" :key="season" class="flex items-center">
            <input 
              type="checkbox"
              v-model="selectedSeasons"
              :value="season"
              @change="updateFilters"
              class="mr-2"
            >
            {{ season }}
          </label>
        </div>
      </div>
      <!-- Бренды -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Бренды</h4>
        <div class="flex flex-col gap-2">
          <label v-for="brand in brands" :key="brand" class="flex items-center">
            <input 
              type="checkbox"
              v-model="selectedBrands"
              :value="brand"
              @change="updateFilters"
              class="mr-2"
            >
            {{ brand }}
          </label>
        </div>
      </div>
      <!-- Цвет -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Цвет</h4>
        <div class="flex flex-col gap-2">
          <label v-for="color in colors" :key="color" class="flex items-center">
            <input 
              type="checkbox"
              v-model="selectedColors"
              :value="color"
              @change="updateFilters"
              class="mr-2"
            >
            {{ color }}
          </label>
        </div>
      </div>
      <!-- Материал верха -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Материал верха</h4>
        <div class="flex flex-col gap-2">
          <label v-for="material in materials" :key="material" class="flex items-center">
            <input 
              type="checkbox"
              v-model="selectedMaterials"
              :value="material"
              @change="updateFilters"
              class="mr-2"
            >
            {{ material }}
          </label>
        </div>
      </div>
      <!-- Особенности модели -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Особенности модели</h4>
        <div class="flex flex-col gap-2">
          <label v-for="feature in features" :key="feature" class="flex items-center">
            <input 
              type="checkbox"
              v-model="selectedFeatures"
              :value="feature"
              @change="updateFilters"
              class="mr-2"
            >
            {{ feature }}
          </label>
        </div>
      </div>
      <!-- Размеры -->
      <div class="mb-6">
        <h4 class="font-medium mb-3">Размеры</h4>
        <div class="grid grid-cols-3 gap-2">
          <label v-for="size in sizes" :key="size" class="flex items-center">
            <input 
              type="checkbox"
              v-model="selectedSizes"
              :value="size"
              @change="updateFilters"
              class="mr-2"
            >
            {{ size }}
          </label>
        </div>
      </div>
      <button 
        @click="updateFilters"
        class="w-full bg-sky-500 text-white py-2 rounded hover:bg-sky-600 transition"
      >
        Показать
      </button>
    </div>
  </template>
<script setup>
import { computed } from 'vue';
import { getFirstImage } from '../utils/imageUtils';

const props = defineProps({
  Productid: Number,
  Name: String,
  Description: String,
  Images: {
    type: [String, Array],
    required: true,
    default: () => ['/sneakers/default-image.jpg'],
    validator(value) {
      if (typeof value === 'string') return true;
      return Array.isArray(value) && value.length > 0;
    }
  },
  Price: Number,
  discountPercent: {
    type: Number,
    default: 20
  },
  isFavorite: Boolean,
  isAdded: Boolean,
  onClickAdd: Function,
  onClickFavorite: Function
})

// Вычисление цены со скидкой
const discountedPrice = computed(() => {
  const originalPrice = props.Price
  if (!originalPrice) return 0
  
  const discount = (originalPrice * props.discountPercent) / 100
  return Math.round(originalPrice - discount)
})

const emit = defineEmits(['openCardModal'])

const visibleFavoriteButton = computed(() => Boolean(props.onClickFavorite))

// Обработчик для открытия модального окна
const handleOpenModal = () => {
  emit('openCardModal')
}

// Обработчики для кнопок с правильной передачей параметров
const handleFavoriteClick = (event) => {
  event.stopPropagation()
  if (props.onClickFavorite) {
    props.onClickFavorite()
  }
}

const handleAddClick = (event) => {
  event.stopPropagation()
  if (props.onClickAdd) {
    props.onClickAdd()
  }
}
</script>

<template>
  <button 
    @click="handleOpenModal"
    class="relative bg-white border-slate-100 rounded-3xl p-8 cursor-pointer transition hover:-translate-y-2 hover:shadow-xl">
    <img
      v-if="visibleFavoriteButton"
      @click.stop="handleFavoriteClick"
      :src="!isFavorite ? '/like-1.svg' : '/like-2.svg'"
      class="absolute top-8 left-8 mb-5"
      alt="Like"
    />
    <img class="w-full object-cover" :src="getFirstImage(props.Images)" :alt="props.Name" />
    <p class="mt-2 text-left">{{ props.Name }}</p>

    <div class="flex justify-between mt-5">
      <div class="flex flex-col items-start">
        <!-- Старая цена (зачеркнутая) -->
        <p class="text-xs text-gray-400 line-through">{{ props.Price }} руб.</p>
        
        <!-- Новая цена со скидкой -->
        <p class="text-lg font-bold text-[#EB5160]">{{ discountedPrice }} руб.</p>
      </div>
      
      <!-- Кнопка добавления в корзину -->
      <img 
        v-if="props.onClickAdd" 
        @click.stop="handleAddClick" 
        :src="!props.isAdded ? '/plus.svg' : '/checked.svg'" 
        alt="Plus" 
        class="cursor-pointer"
      />
    </div>
  </button>
</template>
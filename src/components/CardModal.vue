<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { parseImagePaths } from '../utils/imageUtils';

const props = defineProps({
  item: {
    type: Object,
    required: true
  },
  isFavorite: Boolean,
  isAdded: Boolean
});

const emit = defineEmits(['close', 'add-to-favorite', 'add-to-cart']);

const closeModal = () => {
  emit('close');
};

const selectedSize = ref('');
const isItemInCart = ref(false);

// Проверка, есть ли товар в корзине
const checkIfItemInCart = () => {
  const cartItems = JSON.parse(localStorage.getItem('cart') || '[]');
  isItemInCart.value = cartItems.some(
    item => item.ProductId === props.item.ProductId && item.selectedSize === selectedSize.value
  );
};

const addToCart = () => {
  if (!selectedSize.value) return;
  
  const cartItem = {
    ...props.item,
    selectedSize: selectedSize.value,
    quantity: 1
  };
  
  const cartItems = JSON.parse(localStorage.getItem('cart') || '[]');
  const existingItemIndex = cartItems.findIndex(
    item => item.ProductId === props.item.ProductId && item.selectedSize === selectedSize.value
  );
  
  if (existingItemIndex !== -1) {
    cartItems[existingItemIndex].quantity += 1;
  } else {
    cartItems.push(cartItem);
  }
  
  localStorage.setItem('cart', JSON.stringify(cartItems));
  isItemInCart.value = true;
  
  // Отправляем событие о добавлении товара в корзину
  window.dispatchEvent(new CustomEvent('cart-item-added', { 
    detail: { 
      productId: props.item.ProductId,
      selectedSize: selectedSize.value 
    } 
  }));
  
  emit('add-to-cart', { ...props.item, selectedSize: selectedSize.value });
};

const handleCartItemAdded = (event) => {
  if (props.item && event.detail.productId === props.item.ProductId && 
      event.detail.selectedSize === selectedSize.value) {
    isItemInCart.value = true;
  }
};

const handleCartItemRemoved = (event) => {
  if (props.item && event.detail.productId === props.item.ProductId && 
      event.detail.selectedSize === selectedSize.value) {
    isItemInCart.value = false;
  }
};

onMounted(() => {
  // Проверяем состояние при монтировании
  checkIfItemInCart();
  
  // Добавляем слушатели событий
  window.addEventListener('cart-item-added', handleCartItemAdded);
  window.addEventListener('cart-item-removed', handleCartItemRemoved);
});

onBeforeUnmount(() => {
  // Удаляем слушатели событий
  window.removeEventListener('cart-item-added', handleCartItemAdded);
  window.removeEventListener('cart-item-removed', handleCartItemRemoved);
});

// Преобразование строки размеров в массив, если это необходимо
const sizes = computed(() => {
  if (!props.item?.Sizes) return [];
  return Array.isArray(props.item.Sizes) 
    ? props.item.Sizes 
    : props.item.Sizes.split(',').map(size => size.trim()).filter(Boolean);
});

// Получение массива изображений
const images = computed(() => parseImagePaths(props.item?.Images));
</script>

<template>
  <div class="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center z-50 p-4" @click.self="closeModal">
    <div class="bg-white rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto">
      <!-- Верхняя панель с кнопкой закрытия -->
      <div class="sticky top-0 bg-white z-10 flex justify-end p-4">
        <button @click="closeModal" class="bg-gray-100 hover:bg-gray-200 rounded-full p-2 transition-colors">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>
      
      <!-- Основное содержимое -->
      <div class="flex flex-col md:flex-row p-4 md:p-8 pt-0">
        <!-- Карусель изображений товара -->
        <div class="md:w-1/2 md:pr-8">
          <ImageCarousel :images="images" />
        </div>
        
        <!-- Информация о товаре -->
        <div class="md:w-1/2 mt-6 md:mt-0">
          <h2 class="text-2xl font-bold">{{ item.Name }}</h2>
          <p class="text-gray-600 mt-2">{{ item.Description }}</p>
          
          <div class="mt-6">
            <p class="text-xl font-bold">{{ item.Price }} руб.</p>
          </div>
          
          <!-- Выбор размера -->
          <div class="mt-6">
            <p class="font-medium mb-2">Выберите размер:</p>
            <div class="flex flex-wrap gap-2">
              <button
                v-for="size in sizes"
                :key="size"
                @click="selectedSize = size"
                class="px-4 py-2 border rounded-md transition-colors"
                :class="selectedSize === size ? 'bg-sky-500 text-white' : 'hover:bg-gray-100'"
              >
                {{ size }}
              </button>
            </div>
            <p v-if="!selectedSize && isItemInCart" class="text-red-500 text-sm mt-2">
              Пожалуйста, выберите размер
            </p>
          </div>
          
          <!-- Кнопки действий -->
          <div class="mt-8 flex flex-col space-y-4">
            <button
              @click="addToCart"
              class="w-full py-3 px-6 bg-sky-500 text-white rounded-lg hover:bg-sky-600 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
              :disabled="!selectedSize || isItemInCart"
            >
              {{ isItemInCart ? 'Товар в корзине' : 'Добавить в корзину' }}
            </button>
            
            <button
              @click="$emit('add-to-favorite', item)"
              class="w-full py-3 px-6 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors flex items-center justify-center"
            >
              <img :src="!isFavorite ? '/like-1.svg' : '/like-2.svg'" class="w-5 h-5 mr-2" alt="Like" />
              {{ isFavorite ? 'Убрать из избранного' : 'Добавить в избранное' }}
            </button>
          </div>
          
          <!-- Дополнительная информация о товаре -->
          <div class="mt-8 border-t pt-6">
            <div v-if="item.Brands" class="mb-4">
              <p class="font-medium">Бренд:</p>
              <p class="text-gray-600">{{ item.Brands }}</p>
            </div>
            
            <div v-if="item.Material" class="mb-4">
              <p class="font-medium">Материал:</p>
              <p class="text-gray-600">{{ item.Material }}</p>
            </div>
            
            <div v-if="item.Color" class="mb-4">
              <p class="font-medium">Цвет:</p>
              <p class="text-gray-600">{{ item.Color }}</p>
            </div>
            
            <div v-if="item.Season" class="mb-4">
              <p class="font-medium">Сезон:</p>
              <p class="text-gray-600">{{ item.Season }}</p>
            </div>
            
            <div v-if="item.Features" class="mb-4">
              <p class="font-medium">Особенности:</p>
              <p class="text-gray-600">{{ item.Features }}</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
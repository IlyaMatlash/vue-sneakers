<script setup>
import { defineProps, defineEmits, ref, onMounted, onBeforeUnmount } from 'vue';

const props = defineProps({
  item: Object,
  isFavorite: Boolean,
  isAdded: Boolean,
  onClickAdd: Function,
  onClickFavorite: Function
});

const emit = defineEmits(['close', 'add-to-favorite', 'add-to-cart']);

// Состояние для хранения выбранного размера
const selectedSize = ref(null);
// Состояние для отслеживания добавления в корзину
const isItemInCart = ref(props.isAdded);

const selectSize = (size) => {
  selectedSize.value = size;
  
  // Проверяем, есть ли товар с этим размером в корзине
  checkIfItemInCart();
};

// Проверка наличия товара в корзине
const checkIfItemInCart = () => {
  if (!props.item || !props.item.ProductId) return;
  
  const storageKey = selectedSize.value 
    ? `cartItem_${props.item.ProductId}_${selectedSize.value}` 
    : `cartItem_${props.item.ProductId}`;
  
  isItemInCart.value = localStorage.getItem(storageKey) === 'true';
};

const closeModal = () => {
  emit('close');
};

// Функция для добавления в корзину с выбранным размером
const addToCart = () => {
  if (selectedSize.value) {
    emit('add-to-cart', { ...props.item, selectedSize: selectedSize.value });
    isItemInCart.value = true;
  } else {
    // Можно добавить уведомление о необходимости выбрать размер
    alert('Пожалуйста, выберите размер');
  }
};

// Обработчики событий для синхронизации состояния
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
const sizes = Array.isArray(props.item?.Sizes) 
  ? props.item.Sizes 
  : props.item?.Sizes?.split(',') || [];

// Получение первого изображения, если Images - это массив
const mainImage = Array.isArray(props.item?.Images) 
  ? props.item.Images[0] 
  : props.item?.Images;

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
        <!-- Изображение товара -->
        <div class="md:w-1/2 md:pr-8">
          <div class="bg-gray-50 rounded-xl overflow-hidden">
            <img :src="mainImage" :alt="item.Name" class="w-full h-auto object-contain aspect-square">
          </div>
        </div>
        
        <!-- Информация о товаре -->
        <div class="md:w-1/2 flex flex-col mt-6 md:mt-0">
          <h2 class="text-2xl font-bold mb-2">{{ item.Name }}</h2>
          <p class="text-2xl font-bold mb-6">{{ item.Price }} ₽</p>
          
          <!-- Выбор размера -->
          <div class="mb-6">
            <h3 class="text-sm font-medium mb-3">Выберите размер</h3>
            <div class="grid grid-cols-3 sm:grid-cols-4 gap-2">
              <button 
                v-for="size in sizes" 
                :key="size" 
                @click="selectSize(size)"
                :class="[
                  'border rounded-lg py-3 text-center transition-colors',
                  selectedSize === size 
                    ? 'border-black bg-black text-white' 
                    : 'border-gray-300 hover:border-gray-400'
                ]"
              >
                {{ size }}
              </button>
            </div>
          </div>
          
          <!-- Кнопки действий -->
          <div class="flex flex-col gap-3 mt-auto">
            <button 
              @click="addToCart" 
              class="w-full bg-sky-500 text-white py-2 rounded-lg hover:bg-sky-600 transition"
            >
              {{ isItemInCart ? 'Убрать из корзины' : 'Добавить в корзину' }}
            </button>
            
            <div class="flex gap-3">
              <button 
                @click="$emit('add-to-favorite', item)"
                class="flex items-center justify-center gap-2 border border-gray-300 rounded-lg py-3 px-6 hover:border-gray-400 transition-colors flex-1"
              >
                <img
                  :src="!isFavorite ? '/like-1.svg' : '/like-2.svg'"
                  class="w-5 h-5"
                  alt="Like"
                />
                <span>{{ isFavorite ? 'В избранном' : 'В избранное' }}</span>
              </button>
            </div>
          </div>
          
          <!-- Детали товара -->
          <div class="mt-8 border-t border-gray-200 pt-6">
            <h3 class="text-sm font-medium mb-4">Детали товара</h3>
            <div class="space-y-3 text-sm">
              <p><span class="text-gray-500">Артикул:</span> {{ item.ProductId }}</p>
              <p><span class="text-gray-500">Категория:</span> {{ item.Category }}</p>
              <p v-if="item.Season"><span class="text-gray-500">Сезон:</span> {{ item.Season }}</p>
              <p v-if="item.Color"><span class="text-gray-500">Цвет:</span> {{ item.Color }}</p>
              <p v-if="item.Material"><span class="text-gray-500">Материал:</span> {{ item.Material }}</p>
              <p v-if="item.Features"><span class="text-gray-500">Особенности:</span> {{ item.Features }}</p>
            </div>
          </div>
          
          <!-- Описание товара -->
          <div class="mt-6 border-t border-gray-200 pt-6">
            <h3 class="text-sm font-medium mb-4">Описание</h3>
            <p class="text-sm text-gray-600">{{ item.Description }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
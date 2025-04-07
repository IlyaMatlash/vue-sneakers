<script setup>
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
  item: Object,
  isFavorite: Boolean,
  isAdded: Boolean,
  onClickAdd: Function,
  onClickFavorite: Function
});

const emit = defineEmits(['close', 'add-to-favorite', 'add-to-cart']);


const closeModal = () => {
  emit('close');
};
</script>

<template>
  <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="closeModal">
    <div class="bg-white p-8 rounded-lg max-w-4xl max-h-4xl w-full">
      <div class="flex justify-between items-start mb-4">
        <h2 class="text-xl font-medium mb-8">{{ item.Name }}</h2>
        <button @click="closeModal" class="text-gray-500 hover:text-gray-700">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>
      <div class="flex flex-col md:flex-row">
        <img :src="item.Image" :alt="item.Name" class="w-full md:w-1/2 object-cover rounded-lg mb-4 md:mb-0 md:mr-14">
        <div class="flex flex-col justify-between">
          <div>
            <p class="text-gray-600 mb-5">{{ item.Description }}</p>
            <p class="font-bold text-xl mb-2">Цена: {{ item.Price }} руб.</p>
            <p class="text-sm text-gray-500 mb-4">Артикул: {{ item.ProductId }}</p>
          </div>
          <div class="relative flex justify-between items-center">
            <button @click="$emit('add-to-cart', item)" class="bg-sky-500 text-white px-10 py-2 rounded hover:bg-sky-600 transition">
              {{ isAdded ? 'Убрать из корзины' : 'Добавить в корзину' }}
          </button>
          <button 
            @click="$emit('add-to-favorite', item)"
          >
            <img

              :src="!isFavorite ? '/like-1.svg' : '/like-2.svg'"
              class="w-10 h-10"
              alt="Like"
            />
          </button>
          </div>
          
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import { ref, computed, onMounted, onBeforeUnmount } from 'vue';
import { parseImagePaths } from '../utils/imageUtils';

const props = defineProps({
  images: {
    type: [String, Array],
    required: true
  }
});

const currentIndex = ref(0);
const touchStartX = ref(0);
const touchEndX = ref(0);
const isTransitioning = ref(false);

const imageArray = computed(() => parseImagePaths(props.images));

const goToNext = () => {
  if (isTransitioning.value) return;
  isTransitioning.value = true;
  currentIndex.value = (currentIndex.value + 1) % imageArray.value.length;
  setTimeout(() => {
    isTransitioning.value = false;
  }, 300);
};

const goToPrev = () => {
  if (isTransitioning.value) return;
  isTransitioning.value = true;
  currentIndex.value = (currentIndex.value - 1 + imageArray.value.length) % imageArray.value.length;
  setTimeout(() => {
    isTransitioning.value = false;
  }, 300);
};

const goToSlide = (index) => {
  if (isTransitioning.value) return;
  isTransitioning.value = true;
  currentIndex.value = index;
  setTimeout(() => {
    isTransitioning.value = false;
  }, 300);
};

const handleTouchStart = (e) => {
  touchStartX.value = e.touches[0].clientX;
};

const handleTouchMove = (e) => {
  touchEndX.value = e.touches[0].clientX;
};

const handleTouchEnd = () => {
  if (touchStartX.value - touchEndX.value > 50) {
    // Свайп влево - следующее изображение
    goToNext();
  } else if (touchEndX.value - touchStartX.value > 50) {
    // Свайп вправо - предыдущее изображение
    goToPrev();
  }
};

const handleKeyDown = (e) => {
  if (e.key === 'ArrowLeft') {
    goToPrev();
  } else if (e.key === 'ArrowRight') {
    goToNext();
  }
};

onMounted(() => {
  window.addEventListener('keydown', handleKeyDown);
});

onBeforeUnmount(() => {
  window.removeEventListener('keydown', handleKeyDown);
});
</script>

<template>
  <div class="relative w-full">
    <!-- Основное изображение -->
    <div 
      class="relative overflow-hidden rounded-xl bg-gray-50"
      @touchstart="handleTouchStart"
      @touchmove="handleTouchMove"
      @touchend="handleTouchEnd"
    >
      <div 
        class="flex transition-transform duration-300 ease-in-out h-full"
        :style="{ transform: `translateX(-${currentIndex * 100}%)` }"
      >
        <div 
          v-for="(image, index) in imageArray" 
          :key="index" 
          class="w-full flex-shrink-0 object-cover hover:scale-105 transition duration-300"
        >
          <img 
            :src="image" 
            :alt="`Изображение ${index + 1}`" 
            class="w-full h-auto object-contain aspect-square"
          />
        </div>
      </div>
      
      <!-- Кнопки навигации (только если больше одного изображения) -->
      <template v-if="imageArray.length > 1">
        <button 
          @click.stop="goToPrev" 
          class="absolute left-2 top-1/2 transform -translate-y-1/2 bg-white bg-opacity-70 hover:bg-opacity-100 rounded-full p-2 shadow-md transition-all z-10"
          :disabled="isTransitioning"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
          </svg>
        </button>
        
        <button 
          @click.stop="goToNext" 
          class="absolute right-2 top-1/2 transform -translate-y-1/2 bg-white bg-opacity-70 hover:bg-opacity-100 rounded-full p-2 shadow-md transition-all z-10"
          :disabled="isTransitioning"
        >
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
          </svg>
        </button>
      </template>
    </div>
    
    <!-- Индикаторы (точки) -->
    <div v-if="imageArray.length > 1" class="flex justify-center mt-4 space-x-2">
      <button 
        v-for="(_, index) in imageArray" 
        :key="index"
        @click.stop="goToSlide(index)"
        class="w-3 h-3 rounded-full transition-all focus:outline-none"
        :class="index === currentIndex ? 'bg-sky-500' : 'bg-gray-300 hover:bg-gray-400'"
        :aria-label="`Перейти к изображению ${index + 1}`"
      ></button>
    </div>
    
    <!-- Миниатюры (опционально для больших экранов) -->
    <div v-if="imageArray.length > 1" class="hidden md:flex mt-4 space-x-2 overflow-x-auto">
      <button 
        v-for="(image, index) in imageArray" 
        :key="index"
        @click.stop="goToSlide(index)"
        class="w-16 h-16 rounded-md overflow-hidden transition-all border-2 focus:outline-none"
        :class="index === currentIndex ? 'border-gray-800' : 'border-transparent hover:border-gray-300'"
      >
        <img :src="image" :alt="`Миниатюра ${index + 1}`" class="w-full h-full object-cover" />
      </button>
    </div>
  </div>
</template>
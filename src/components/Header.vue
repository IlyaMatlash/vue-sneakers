<script setup>
import { ref } from 'vue'
defineProps({
  cartItemsCount: Number,
  items: Array
})
const isMenuOpen = ref(false)
const emit = defineEmits(['openDrawer'])
const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value
}
</script>
<template>
  <header class="flex flex-col md:flex-row justify-between items-center border-b border-slate-100 px-4 md:px-10 py-4 md:py-8">
    <div class="flex justify-between items-center w-full md:w-auto">
      <router-link to="/" class="mb-4 md:mb-0">
        <div class="flex items-center gap-4">
          <img class="w-[80px] md:w-[100px]" src="/logo.svg" alt="Logo" />
        </div>
      </router-link>
      
      <!-- Burger Menu Button -->
      <button 
        class="md:hidden p-2 text-gray-500 hover:text-gray-800"
        @click="toggleMenu"
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path v-if="!isMenuOpen" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/>
          <path v-else stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
        </svg>
      </button>
    </div>
    <!-- Navigation Menu -->
    <ul :class="['flex flex-col md:flex-row items-center gap-4 md:gap-10', 
      isMenuOpen ? 'block w-full' : 'hidden md:flex']"
    >
      <li
        @click="() => emit('openDrawer')"
        class="flex relative items-center gap-2 md:gap-3 text-gray-500 hover:text-black cursor-pointer w-full md:w-auto py-2 md:py-0"
      >
        <img class="w-6 md:w-7" src="/cart.svg" alt="Cart" />
        <div class="absolute -top-1 -right-2 md:top-4 md:left-7 text-sky-500 hover:text-sky-600 hidden md:inline">{{ cartItemsCount }}</div>
        <span class="md:hidden">Корзина</span>
      </li>
      <router-link to="/favorites" class="w-full md:w-auto">
        <li class="flex items-center gap-2 md:gap-3 text-gray-500 hover:text-black cursor-pointer py-2 md:py-0">
          <img class="w-6 md:w-7" src="/heart.svg" alt="Cart" />
          <span>Избранные</span>
        </li>
      </router-link>
      <router-link to="/shoe-care" class="w-full md:w-auto">
        <li class="flex items-center gap-2 md:gap-3 text-gray-500 hover:text-black cursor-pointer w-full md:w-auto py-2 md:py-0">
          <img class="w-6 md:w-7" src="/step.svg" alt="Cart" />
          <span>Уход за обувью</span>
        </li>
      </router-link>
      
    </ul>
  </header>
</template>
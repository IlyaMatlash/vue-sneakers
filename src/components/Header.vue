<script setup>
defineProps({
  cartItemsCount: Number,
  items: Array
})

const emit = defineEmits(['openDrawer'])
</script>

<template>
  <header class="flex justify-between border-b border-slate-100 px-10 py-8">
    <router-link to="/">
      <div class="flex items-center gap-4">
        <img class="w-100" src="/logo.svg" alt="Logo" />
      </div>
    </router-link>

    <ul class="flex items-center gap-10">
      <li
        @click="() => emit('openDrawer')"
        class="flex relative items-center gap-3 text-gray-500 hover:text-black cursor-pointer"
      >
        <img class="w-7" src="/cart.svg" alt="Cart" />
        <div class="absolute top-4 left-7 text-sky-500 hover:text-sky-600">{{ cartItemsCount }}</div>
      </li>
      <router-link to="/favorites">
        <li class="flex items-center gap-3 text-gray-500 hover:text-black cursor-pointer">
          <img class="w-7" src="/heart.svg" alt="Cart" />
          <span>Избранные</span>
        </li>
      </router-link>
      <li class="flex items-center gap-3 text-gray-500 hover:text-black cursor-pointer">
        <img class="w-7" src="/step.svg" alt="Cart" />
        <span>Уход за обувью</span>
      </li>
    </ul>
  </header>
</template>

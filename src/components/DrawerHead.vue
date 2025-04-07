<script setup>
import { inject } from 'vue'

const { closeDrawer } = inject('cart')
</script>

<template>
  <div class="flex items-flex justify-between gap-5 mb-8">
    <h2 class="text-3xl font-signate">Корзина</h2>
    <svg
      @click="() => closeDrawer()"
      class="opacity-30 cursor-pointer transition hover:opacity-100"
      width="32"
      height="32"
      viewBox="0 0 16 16"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
    >
    <path
        d="M12.0799 10.6155L9.6311 8.16673L12.0798 5.71801C12.3732 5.42462 12.3732 4.95072 12.0798 4.65733C11.7864 4.36394 11.3125 4.36394 11.0191 4.65733L8.57043 7.10605L6.12177 4.65739C5.82838 4.364 5.35448 4.364 5.06109 4.65739C4.7677 4.95078 4.7677 5.42468 5.06109 5.71807L7.50975 8.16673L5.06104 10.6154C4.76765 10.9088 4.76765 11.3827 5.06104 11.6761C5.35443 11.9695 5.82833 11.9695 6.12172 11.6761L8.57043 9.22741L11.0193 11.6763C11.3127 11.9697 11.7866 11.9697 12.08 11.6763C12.3734 11.3829 12.3734 10.909 12.0799 10.6155Z"
        fill="black"
      />
    </svg>
    
  </div>
</template>

<script setup>
import { inject } from 'vue'
import CartItem from './CartItem.vue'

const { cart, removeFromCart } = inject('cart')
</script>

<template>
  <div class="flex flex-col flex-1 gap-4" v-auto-animate>
    <CartItem
      v-for="item in cart"
      :key="item.id"
      :name="item.name"
      :price="item.price"
      :image="item.image"
      @on-click-remove="() => removeFromCart(item)"
    />
  </div>
</template>

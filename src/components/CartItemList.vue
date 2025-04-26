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
      :Name="item.Name"
      :Price="item.Price"
      :Image="item.Image"
      :quantity="item.quantity"
      v-model:quantity="item.quantity"
      @on-click-remove="() => removeFromCart(item)"
    />
  </div>
</template>

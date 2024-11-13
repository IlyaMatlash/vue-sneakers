<script setup>
import { inject } from 'vue'
import CartItem from './CartItem.vue'

const { cart, removeFromCart } = inject('cart')
</script>

<template>
  <div class="flex flex-col flex-1 gap-4" v-auto-animate>
    <CartItem
      v-for="item in cart"
      :key="item.ProductId"
      :name="item.Name"
      :price="item.Price"
      :image="item.Image"
      @on-click-remove="() => removeFromCart(item)"
    />
  </div>
</template>

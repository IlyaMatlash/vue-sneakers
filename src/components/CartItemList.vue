<script setup>
import { inject } from 'vue'
import CartItem from './CartItem.vue'

const { cart, removeFromCart } = inject('cart')
</script>

<template>
  <div class="flex flex-col flex-1 gap-2 sm:gap-4 px-2 sm:px-0" v-auto-animate>
    <CartItem
      v-for="item in cart"
      :key="item.id"
      :Name="item.Name"
      :Price="item.Price"
      :Images="item.Images"
      :quantity="item.quantity"
      :Sizes="item.Sizes"
      v-model:quantity="item.quantity"
      @on-click-remove="() => removeFromCart(item)"
    />
  </div>
</template>

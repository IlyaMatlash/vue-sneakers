<script setup>
import CartItemCalculator from './CartItemCalculator.vue';
import { getFirstImage } from '../utils/imageUtils';

const emit = defineEmits(['onClickRemove', 'update:quantity', 'update:totalPrice'])

defineProps({
  Productid: Number,
  Name: String,
  Description: String,
  Images: {
    type: [String, Array],
    default: () => []
  },
  Price: Number,
  quantity: {
    type: Number,
    default: 1
  }
})
</script>

<template>
  <div class="flex border border-slate-200 p-4 rounded-xl gap-4">
    <img 
      class="w-20 h-16 object-cover rounded-lg" 
      :src="getFirstImage(Images)"
      :alt="Name"
    />

    <div class="flex flex-col flex-1">
      <p>{{ Name }}</p>

      <div class="flex justify-between mt-2">
        <CartItemCalculator
          :price="Price"
          :initial-quantity="quantity"
          :min-quantity="1"
          @update:quantity="$emit('update:quantity', $event)"
          @update:totalPrice="$emit('update:totalPrice', $event)"
        />
        <img
          @click="emit('onClickRemove')"
          class="opacity-40 hover:opacity-100 cursor-pointer transition"
          src="/close.svg"
          alt="Close"
        />
      </div>
    </div>
  </div>
</template>
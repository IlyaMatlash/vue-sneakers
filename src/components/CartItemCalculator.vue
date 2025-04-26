<script setup>
import { ref, watch } from 'vue'
const props = defineProps({
  initialQuantity: {
    type: Number,
    default: 1
  },
  price: {
    type: Number,
    required: true
  },
  minQuantity: {
    type: Number,
    default: 1
  }
})
const emit = defineEmits(['update:quantity', 'update:totalPrice'])
const quantity = ref(props.initialQuantity)
const totalPrice = ref(props.price * props.initialQuantity)
const updateQuantity = (newQuantity) => {
  if (newQuantity < props.minQuantity) {
    quantity.value = props.minQuantity
  } else {
    quantity.value = newQuantity
  }
  calculateTotal()
}
const calculateTotal = () => {
  totalPrice.value = quantity.value * props.price
  emit('update:quantity', quantity.value)
  emit('update:totalPrice', totalPrice.value)
}
watch(() => props.price, (newPrice) => {
  totalPrice.value = quantity.value * newPrice
})
</script>
<template>
  <div class="flex items-center gap-4">
    <div class="flex items-center border rounded-lg">
      <button 
        @click="updateQuantity(quantity - 1)"
        class="px-3 py-1 text-gray-600 hover:bg-gray-100"
        :disabled="quantity <= minQuantity"
      >–</button>
      <span class="px-3">{{ quantity }}</span>
      <button 
        @click="updateQuantity(quantity + 1)"
        class="px-3 py-1 text-gray-600 hover:bg-gray-100"
      >+</button>
    </div>
    <div class="font-medium">{{ totalPrice }} руб.</div>
  </div>
</template>
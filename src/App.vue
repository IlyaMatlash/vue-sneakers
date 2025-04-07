<script setup>
import { ref, provide, watch, computed, onMounted } from 'vue'
import Header from './components/Header.vue'
import Drawer from './components/Drawer.vue'

// Корзина (START)
const cart = ref([])
const drawerOpen = ref(false)
const totalPrice = computed(() => cart.value.reduce((acc, item) => acc + item.Price, 0))
const vatPrice = computed(() => Math.round((totalPrice.value * 20) / 100))
const cartItemsCount = computed(() => cart.value.length)

const closeDrawer = () => {
  drawerOpen.value = false
}
const openDrawer = () => {
  drawerOpen.value = true
}

const addToCart = (item) => {
  if (!item || !item.ProductId) {
    console.error('Invalid item provided to addToCart');
    return;
  }
  if (!cart.value.some(cartItem => cartItem.ProductId === item.ProductId)) {
    cart.value.push(item);
    item.isAdded = true;
    localStorage.setItem('cartItems', JSON.stringify(cart.value));
    localStorage.setItem(`cartItem_${item.ProductId}`, 'true');
  }
}
const removeFromCart = (item) => {
  if (!item || !item.ProductId) {
    console.error('Invalid item provided to removeFromCart');
    return;
  }
  try {
    cart.value = cart.value.filter(cartItem => cartItem.ProductId !== item.ProductId);
    
    // Update isAdded state in all relevant components
    const event = new CustomEvent('cart-item-removed', { 
      detail: { productId: item.ProductId }
    });
    window.dispatchEvent(event);
    
    if (cart.value.length === 0) {
      localStorage.removeItem('cartItems');
    } else {
      localStorage.setItem('cartItems', JSON.stringify(cart.value));
    }
    localStorage.removeItem(`cartItem_${item.ProductId}`);
  } catch (error) {
    console.error('Error removing item from cart:', error);
  }
}

watch(
  cart,
  (newCart) => {
    if (newCart.length === 0) {
      localStorage.removeItem('cartItems');
    } else {
      localStorage.setItem('cartItems', JSON.stringify(newCart));
    }
  },
  { deep: true }
)

onMounted(() => {
  const savedCart = localStorage.getItem('cartItems')
  if (savedCart) {
    cart.value = JSON.parse(savedCart)
    cart.value.forEach(item => {
      if (localStorage.getItem(`cartItem_${item.ProductId}`) === 'true') {
        item.isAdded = true
      }
    })
  }
})

provide('cart', {
  cart,
  closeDrawer,
  openDrawer,
  addToCart,
  removeFromCart
})
//Корзина (END)

</script>

<template>
  <Drawer
    v-if="drawerOpen"
    :total-price="totalPrice"
    :vat-price="vatPrice"
  />
  <div class="bg-white m-auto rounded-xl shadow-xl mt-14">
    <Header :cart-items-count="cartItemsCount" @open-drawer="openDrawer" />

    <div class="p-10">
      <router-view></router-view>
    </div>
  </div>
</template>

<style scoped></style>

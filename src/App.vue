<script setup>
import { ref, provide, watch, computed, onMounted} from 'vue'
import Header from './components/Header.vue'
import Drawer from './components/Drawer.vue'
import Footer from './components/Footer.vue'

// Корзина (START)
const cart = ref([])
const drawerOpen = ref(false)
const totalPrice = computed(() => cart.value.reduce((acc, item) => acc + (item.Price * (item.quantity || 1)), 0))
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
  const cartItem = {
    ...item,
    quantity: 1,
    isAdded: true
  };
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
      totalPrice.value = newCart.reduce((acc, item) => acc + (item.Price * (item.quantity || 1)), 0);
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
  <div class="bg-white m-auto shadow-xl min-h-screen flex flex-col">
    <Header class="fixed top-0 left-0 right-0 z-20 bg-white shadow-md" :cart-items-count="cartItemsCount" @open-drawer="openDrawer" />
    <div class="p-10 mt-[140px]">
      <router-view></router-view>
    </div>
  </div>
  <Footer/>
</template>

<style scoped></style>

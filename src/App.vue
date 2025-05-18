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
  
  // Создаем новый объект для корзины с нужными свойствами
  const cartItem = {
    ...item,
    quantity: 1,
    isAdded: true,
    // Сохраняем выбранный размер, если он есть
    selectedSize: item.selectedSize || null
  };
  
  // Проверяем, есть ли уже такой товар в корзине
  const existingItemIndex = cart.value.findIndex(
    cartItem => cartItem.ProductId === item.ProductId && 
    cartItem.selectedSize === item.selectedSize
  );
  
  if (existingItemIndex !== -1) {
    // Если товар уже есть в корзине, увеличиваем его количество
    cart.value[existingItemIndex].quantity += 1;
  } else {
    // Если товара нет в корзине, добавляем его
    cart.value.push(cartItem);
  }
  
  // Устанавливаем флаг isAdded для оригинального объекта
  // для корректного отображения в UI
  item.isAdded = true;
  
  // Сохраняем обновленную корзину в localStorage
  localStorage.setItem('cartItems', JSON.stringify(cart.value));
  
  // Сохраняем информацию о товаре с учетом размера
  const storageKey = item.selectedSize 
    ? `cartItem_${item.ProductId}_${item.selectedSize}` 
    : `cartItem_${item.ProductId}`;
  localStorage.setItem(storageKey, 'true');
  
  // Оповещаем другие компоненты о добавлении товара
  const event = new CustomEvent('cart-item-added', { 
    detail: { 
      productId: item.ProductId,
      selectedSize: item.selectedSize 
    }
  });
  window.dispatchEvent(event);
}

const removeFromCart = (item) => {
  if (!item || !item.ProductId) {
    console.error('Invalid item provided to removeFromCart');
    return;
  }
  try {
    // Удаляем товар с учетом размера
    cart.value = cart.value.filter(cartItem => 
      !(cartItem.ProductId === item.ProductId && cartItem.selectedSize === item.selectedSize)
    );
    
    // Оповещаем другие компоненты об удалении товара
    const event = new CustomEvent('cart-item-removed', { 
      detail: { 
        productId: item.ProductId,
        selectedSize: item.selectedSize 
      }
    });
    window.dispatchEvent(event);
    
    // Обновляем localStorage
    if (cart.value.length === 0) {
      localStorage.removeItem('cartItems');
    } else {
      localStorage.setItem('cartItems', JSON.stringify(cart.value));
    }
    
    // Удаляем информацию о товаре с учетом размера
    const storageKey = item.selectedSize 
      ? `cartItem_${item.ProductId}_${item.selectedSize}` 
      : `cartItem_${item.ProductId}`;
    localStorage.removeItem(storageKey);
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
      // Проверяем наличие товара в localStorage с учетом размера
      const storageKey = item.selectedSize 
        ? `cartItem_${item.ProductId}_${item.selectedSize}` 
        : `cartItem_${item.ProductId}`;
      if (localStorage.getItem(storageKey) === 'true') {
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

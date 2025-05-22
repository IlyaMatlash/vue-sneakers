import './assets/main.css'
import { createApp, reactive } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { autoAnimatePlugin } from '@formkit/auto-animate/vue'
import { useFavorites } from './composables/useFavorites'
import App from './App.vue'
import Home from './pages/Home.vue'
import Favorites from './pages/Favorites.vue'
import Admin from './pages/Admin.vue'
import AnswersPage from './pages/AnswersPage.vue'

import ImageCarousel from './components/ImageCarousel.vue'
const app = createApp(App)

const cart = reactive({
  value: [],
  isOpen: false,
  openDrawer() {
    this.isOpen = true
  },
  closeDrawer() {
    this.isOpen = false
  }
})

const favoritesService = useFavorites()
app.provide('favorites', favoritesService)

const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/favorites', name: 'Favorites', component: Favorites },
  { path: '/admin', name: 'Admin', component: Admin },
  { path: '/answers', name: 'Answers', component: AnswersPage },
  {path: '/about', name: 'About', component: () => import('@/components/AboutUs.vue')},
  { path: '/delivery', name: 'Delivery', component: () => import('@/pages/DeliveryPage.vue')},
  { path: '/shoe-care', name: 'ShoeCare', component: () => import('@/pages/ShoeCarePage.vue')}
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior() {
    return { top: 0 }
  }
})
app.component('ImageCarousel', ImageCarousel)
app.use(router)
app.use(autoAnimatePlugin)

app.mount('#app')

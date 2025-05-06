import './assets/main.css'
import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { autoAnimatePlugin } from '@formkit/auto-animate/vue'
import App from './App.vue'
import Home from './pages/Home.vue'
import Favorites from './pages/Favorites.vue'
import Admin from './pages/Admin.vue'
//import Users from './pages/Users.vue'
import AnswersPage from './pages/AnswersPage.vue'

const app = createApp(App)

const routes = [
  { path: '/', name: 'Home', component: Home },
  { path: '/favorites', name: 'Favorites', component: Favorites },
  { path: '/admin', name: 'Admin', component: Admin },
 // { path: '/users', name: 'Users', component: Users },
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

app.use(router)
app.use(autoAnimatePlugin)

app.mount('#app')

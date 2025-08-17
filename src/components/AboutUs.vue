<script setup>
import { ref } from 'vue'
const contactInfo = ref({
  phone: '8 999 999 99 99',
  email: 'info@sneakers.ru',
  address: 'г. Москва, ул. Пушкина, д. 10',
  workingHours: 'Пн-Вс: 10:00 - 22:00'
})
</script>
<template>
  <div class="max-w-4xl mx-auto py-8">
    <h1 class="text-3xl font-bold font-signate mb-8">О нас</h1>
    
    <div class="bg-white rounded-lg shadow-lg p-8 mb-8">
      <h2 class="text-2xl font-medium mb-6">Контактная информация</h2>
      
      <div class="grid md:grid-cols-2 gap-8">
        <div>
          <h3 class="text-lg font-medium mb-4">Как с нами связаться</h3>
          <ul class="space-y-3">
            <li class="flex items-center">
              <span class="font-medium mr-2">Телефон:</span>
              <a href="tel:89999999999" class="text-sky-500 hover:text-sky-600">{{ contactInfo.phone }}</a>
            </li>
            <li class="flex items-center">
              <span class="font-medium mr-2">Email:</span>
              <a href="mailto:info@sneakers.ru" class="text-sky-500 hover:text-sky-600">{{ contactInfo.email }}</a>
            </li>
            <li class="flex items-center">
              <span class="font-medium mr-2">Адрес:</span>
              <span>{{ contactInfo.address }}</span>
            </li>
            <li class="flex items-center">
              <span class="font-medium mr-2">Режим работы:</span>
              <span>{{ contactInfo.workingHours }}</span>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>
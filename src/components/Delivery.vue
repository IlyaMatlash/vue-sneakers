<script setup>
const deliveryMethods = [
  {
    title: 'Курьерская доставка СДЭК',
    description: 'Доставка по России от 3 до 7 дней',
    price: 'от 300 ₽',
    icon: '/cdek-icon.svg'
  },
  {
    title: 'Почта России',
    description: 'Доставка по России от 5 до 14 дней',
    price: 'от 250 ₽',
    icon: '/post-icon.svg'
  }
]
const deliveryInfo = {
  title: 'Информация о доставке',
  description: 'Мы осуществляем доставку по всей России. Стоимость и сроки доставки зависят от региона и выбранного способа доставки.'
}
</script>
<template>
  <div class="max-w-4xl mx-auto">
    <!-- Информация о доставке -->
    <div class="bg-white rounded-lg shadow-lg p-8 mb-8">
      <h2 class="text-2xl font-medium mb-4">{{ deliveryInfo.title }}</h2>
      <p class="text-gray-600">{{ deliveryInfo.description }}</p>
    </div>
    <!-- Способы доставки -->
    <div class="grid md:grid-cols-2 gap-6">
      <div 
        v-for="method in deliveryMethods" 
        :key="method.title"
        class="bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition duration-300"
      >
        <div class="flex items-start gap-4">
          <img 
            :src="method.icon" 
            :alt="method.title"
            class="w-12 h-12 object-contain"
          />
          <div>
            <h3 class="text-xl font-medium mb-2">{{ method.title }}</h3>
            <p class="text-gray-600 mb-2">{{ method.description }}</p>
            <p class="text-sky-500 font-medium">{{ method.price }}</p>
          </div>
        </div>
      </div>
    </div>
    <!-- Дополнительная информация -->
    <div class="mt-8 bg-white rounded-lg shadow-lg p-8">
      <h2 class="text-2xl font-medium mb-4">Важная информация</h2>
      <ul class="list-disc list-inside space-y-2 text-gray-600">
        <li>Доставка осуществляется только по территории России</li>
        <li>Сроки доставки могут варьироваться в зависимости от региона</li>
        <li>При заказе от 5000 ₽ доставка бесплатная</li>
        <li>Отслеживание заказа доступно в личном кабинете</li>
      </ul>
    </div>
    <div class="relative mt-12">
      <div class="w-screen relative left-1/2 right-1/2 -ml-[50vw] -mr-[50vw]">
        <div class="px-4 sm:px-6 lg:px-8">
          <router-link to="/" class="block">
            <img 
              src="/banner_3.png" 
              alt="Shoe Care Banner"
              class="w-full h-auto object-cover rounded-lg shadow-lg cursor-pointer hover:opacity-90 transition-opacity
                     aspect-[16/9] sm:aspect-[20/9] md:aspect-[24/9] lg:aspect-[28/9] xl:aspect-[32/9]"
            />
          </router-link>
        </div>
      </div>
    </div>
  </div>
</template>
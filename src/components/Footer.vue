vue
<template>
  <footer class="w-full bg-gray-900 mt-auto">
    <div class="mx-auto px-16 py-8">
      <div class="grid grid-cols-1 md:grid-cols-3 gap-8">
        <div>
          <h3 class="text-gray-500 font-medium mb-4">О компании</h3>
          <ul class="space-y-2 text-sky-50">
            <li><router-link to="/about" class="hover:text-sky-400 transition duration-300">О нас</router-link></li>
            <li>
              <router-link to="/answers" class="hover:text-sky-400 transition duration-300">Ответы на вопросы</router-link>
            </li>
          </ul>
        </div>
        <div>
          <h3 class="text-gray-500 font-medium mb-4">Покупателям</h3>
          <ul class="space-y-2 text-sky-50">
            <li><a href="#" class="hover:text-sky-400 transition duration-300">Доставка</a></li>
            <li>
              <a href="#" class="hover:text-sky-400 transition duration-300">Уход за обувью</a>
            </li>
          </ul>
        </div>
        <div>
          <h3 class="text-gray-500 font-medium mb-4">Контакты</h3>
          <ul class="space-y-2 text-sky-50">
            <li>8 800 555 35 35</li>
            <li>info@sneakers.ru</li>
          </ul>
        </div>
      </div>
    </div>
  </footer>
</template>

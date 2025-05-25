<script setup>
const careItems = [
  {
    title: 'Регулярная чистка',
    description: [
      'Очищайте обувь после каждого использования мягкой щеткой или специальной салфеткой.',
      'Для более тщательной очистки используйте специальные чистящие средства, подходящие для материала вашей обуви.'
    ],
    icon: '/cleaning-icon.png'
  },
  {
    title: 'Сушка',
    description: [
      'Сушите обувь естественным путем при комнатной температуре, избегая прямых источников тепла.',
      'Для ускорения процесса можно использовать газеты или специальные впитывающие вкладыши.'
    ],
    icon: '/drying-icon.png'
  },
  {
    title: 'Защита и уход',
    description: [
      'Регулярно обрабатывайте обувь защитными средствами (спреи, кремы, пропитки) для предотвращения попадания влаги и загрязнений.',
      'Это продлит срок службы вашей обуви.'
    ],
    icon: '/protection-icon.png'
  },
  {
    title: 'Хранение',
    description: [
      'Храните обувь в сухом, проветриваемом месте, используя формодержатели для сохранения формы.',
      'Избегайте длительного воздействия прямых солнечных лучей.'
    ],
    icon: '/storage-icon.png'
  },
  {
    title: 'Ремонт и замена',
    description: [
      'Своевременно обращайтесь в ремонт при появлении повреждений.',
      'Регулярно проверяйте состояние подошвы и при необходимости заменяйте её для комфортного использования.'
    ],
    icon: '/repair-icon.png'
  }
]
</script>
<template>
  <div class="max-w-4xl mx-auto">
    <!-- Основная информация -->
    <div class="bg-white rounded-lg shadow-lg p-8 mb-8">
      <h2 class="text-2xl font-medium mb-4">Правила ухода за обувью</h2>
      <p class="text-gray-600">Правильный уход за обувью не только продлевает срок её службы, но и сохраняет первоначальный внешний вид. Следуйте нашим рекомендациям для поддержания обуви в отличном состоянии.</p>
    </div>
    
    <!-- Карточки с рекомендациями -->
    <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
      <!-- Большая карточка на 4 колонки -->
      <div class="md:col-span-4 bg-white rounded-lg shadow-lg p-8 hover:shadow-xl transition duration-300">
        <div class="flex items-start gap-6">
          <img 
            :src="careItems[0].icon" 
            :alt="careItems[0].title"
            class="w-16 h-16 object-contain"
          />
          <div>
            <h3 class="text-xl font-medium mb-4">{{ careItems[0].title }}</h3>
            <div class="space-y-3">
              <p v-for="(paragraph, index) in careItems[0].description" 
                 :key="index" 
                 class="text-gray-600 text-left">
                {{ paragraph }}
              </p>
            </div>
          </div>
        </div>
      </div>
      <!-- Вертикальная карточка на 2 колонки -->
      <div class="md:col-span-2 bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition duration-300">
        <div class="flex flex-col items-center">
          <img 
            :src="careItems[1].icon" 
            :alt="careItems[1].title"
            class="w-16 h-16 object-contain mb-4"
          />
          <h3 class="text-xl font-medium mb-3">{{ careItems[1].title }}</h3>
          <div class="space-y-2">
            <p v-for="(paragraph, index) in careItems[1].description" 
               :key="index" 
               class="text-gray-600 text-left">
              {{ paragraph }}
            </p>
          </div>
        </div>
      </div>
      <!-- Средние карточки на 2 колонки каждая -->
      <div 
        v-for="item in careItems.slice(2)" 
        :key="item.title"
        class="md:col-span-2 bg-white rounded-lg shadow-lg p-6 hover:shadow-xl transition duration-300"
      >
        <div class="flex flex-col items-center">
          <img 
            :src="item.icon" 
            :alt="item.title"
            class="w-16 h-16 object-contain mb-3"
          />
          <h3 class="text-xl text-left font-medium mb-3">{{ item.title }}</h3>
          <div class="space-y-2">
            <p v-for="(paragraph, index) in item.description" 
               :key="index" 
               class="text-gray-600">
              {{ paragraph }}
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div class="relative mt-12">
      <div class="w-screen relative left-1/2 right-1/2 -ml-[50vw] -mr-[50vw]">
        <div class="px-4 sm:px-6 lg:px-8">
          <router-link to="/" class="block">
            <img 
              src="/banner_2.png" 
              alt="Shoe Care Banner"
              class="w-full h-auto object-cover rounded-lg shadow-lg cursor-pointer hover:opacity-90 transition-opacity
                     aspect-[16/9] sm:aspect-[20/9] md:aspect-[24/9] lg:aspect-[28/9] xl:aspect-[32/9]"
            />
          </router-link>
        </div>
      </div>
    </div>
</template>
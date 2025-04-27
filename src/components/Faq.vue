<script setup>
import { ref } from 'vue'
const faqs = ref([
  {
    id: 1,
    question: 'Как оформить заказ?',
    answer:
      'Чтобы оформить заказ, добавьте товары в корзину, перейдите в корзину, заполните форму доставки и нажмите "Оформить заказ".',
    isOpen: false
  },
  {
    id: 2,
    question: 'Какие способы доставки доступны?',
    answer:
      'Мы осуществляем доставку через CDEK и Почту России. При оформлении заказа вы можете выбрать удобный для вас способ доставки.',
    isOpen: false
  },
  {
    id: 3,
    question: 'Какие способы оплаты доступны?',
    answer: 
      'На данный момент в нашем магазине можно оплатить заказ онлайн или указать "Оплата при получении" при выборе способа доставки.',
    isOpen: false
  },
  {
    id: 4,
    question: 'Как отследить заказ?',
    answer:
      'После отправки заказа мы вышлем вам на эл. почту и в смс-сообщении информацию, содержащую трек-номер, а также ссылку на сайт курьерской службы СДЭК, где можно будет отследить перемещение вашего заказа. В случае, если статус посылки не меняется более недели, пожалуйста, позвоните в ближайшее отделение курьерской службы СДЭК и уточните дополнительную информацию. При возникновении дополнительных трудностей обратитесь к нашей службе клиентской поддержки по номеру телефона +7 (800) 555-35-35.',
    isOpen: false
  },
  {
    id: 5,
    question: 'Гарантирована ли безопасность моих данных?',
    answer: 
      'Мы гарантируем полную безопасность ваших персональных данных. Если у вас есть вопросы, пожалуйста, ознакомьтесь с нашей Политикой конфиденциальности.',
    isOpen: false
  },
])
const toggleFaq = (id) => {
  faqs.value = faqs.value.map((faq) => ({
    ...faq,
    isOpen: faq.id === id ? !faq.isOpen : false
  }))
}
</script>
<template>
  <div class="w-full mx-auto">
    <div class="space-y-4 w-full">
      <div
        v-for="faq in faqs"
        :key="faq.id"
        class="border border-slate-200 rounded-lg overflow-hidden"
      >
        <button
          @click="toggleFaq(faq.id)"
          class="flex justify-between w-full p-4 text-left bg-white hover:bg-slate-50 transition-colors"
        >
          <span class="font-medium text-left w-full">{{ faq.question }}</span>
          <svg
            class="w-5 h-5 transition-transform duration-300"
            :class="{ 'rotate-180': faq.isOpen }"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              stroke-linecap="round"
              stroke-linejoin="round"
              stroke-width="2"
              d="M19 9l-7 7-7-7"
            />
          </svg>
        </button>
        <div 
          class="answer-wrapper" 
          :class="{ 'answer-wrapper--open': faq.isOpen }"
        >
          <p class="p-4 bg-slate-50 border-t border-slate-200 text-gray-600">
            {{ faq.answer }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
<style scoped>
.answer-wrapper {
  max-height: 0;
  opacity: 0;
  overflow: hidden;
  transition: all 0.3s ease-in-out;
}
.answer-wrapper--open {
  max-height: 500px;
  opacity: 1;
}
</style>

<script setup>
import axios from 'axios'
import { ref, inject, computed } from 'vue'
import DrawerHead from './DrawerHead.vue'
import CartItemList from './CartItemList.vue'
import InfoBlock from './InfoBlock.vue'

const props = defineProps({
  totalPrice: Number,
  vatPrice: Number
})

const { cart, closeDrawer } = inject('cart')

const isCreating = ref(false)
const orderId = ref(null)
const recipientName = ref('')
const recipientAddress = ref('')
const postalCode = ref('')
const phoneNumber = ref('')
const email = ref('')
const promoCode = ref('')
const deliveryMethod = ref('')
const formSubmited = ref(false)
const errorMessage = ref('')

const createOrder = async () => {
  formSubmited.value = true;
  if (!isFormValid.value || cartIsEmpty.value) return;

  try {
    isCreating.value = true;
    errorMessage.value = '';

    // Формируем данные заказа в соответствии с моделью OrderDto
    const orderData = {
      orderItems: cart.value.map(item => ({
        productId: item.id || item.ProductId,
        quantity: item.quantity,
        price: item.Price || item.price,
        name: item.Name || item.name
      })),
      totalPrice: props.totalPrice,
      recipientName: recipientName.value,
      recipientAddress: recipientAddress.value,
      postalCode: postalCode.value,
      phoneNumber: phoneNumber.value,
      email: email.value,
      promoCode: promoCode.value || null,
      deliveryMethod: deliveryMethod.value,
      orderDate: new Date().toISOString(),
      status: "New"
    };

    const response = await axios.post('http://localhost:5072/api/orders', orderData, {
      headers: {
        'Content-Type': 'application/json'
      }
    });

    if (response.status === 201 || response.status === 200) {
      cart.value = [];
      orderId.value = response.data.id;
      resetForm();
      localStorage.setItem('lastOrderId', orderId.value);
      setTimeout(() => {
        closeDrawer();
      }, 5000);
    } else {
      throw new Error('Неожиданный ответ от сервера');
    }
  } catch (error) {
    console.error('Ошибка при создании заказа:', error);
    
    if (error.response) {
      if (error.response.status === 400) {
        if (error.response.data.errors) {
          const validationErrors = Object.values(error.response.data.errors).flat();
          errorMessage.value = validationErrors.join(', ');
        } else {
          errorMessage.value = error.response.data.title || error.response.data.message || 'Ошибка валидации данных';
        }
      } else if (error.response.status === 500) {
        errorMessage.value = 'Произошла внутренняя ошибка сервера. Пожалуйста, попробуйте позже.';
      } else if (error.response.status === 404) {
        errorMessage.value = 'Сервис оформления заказов временно недоступен. Пожалуйста, попробуйте позже.';
      } else {
        errorMessage.value = error.response.data.message || 'Произошла ошибка при оформлении заказа';
      }
    } else if (error.request) {
      errorMessage.value = 'Сервер не отвечает. Пожалуйста, проверьте подключение к интернету.';
    } else {
      errorMessage.value = 'Произошла ошибка при оформлении заказа. Пожалуйста, попробуйте снова.';
    }
  } finally {
    isCreating.value = false;
  }
};

const resetForm = () => {
  recipientName.value = '';
  recipientAddress.value = '';
  postalCode.value = '';
  phoneNumber.value = '';
  email.value = '';
  promoCode.value = '';
  deliveryMethod.value = '';
  formSubmited.value = false;
};

const cartIsEmpty = computed(() => cart.value.length === 0);
const ButtonDisabled = computed(() => isCreating.value || cartIsEmpty.value || !isFormValid.value);

const isFormValid = computed(() => {
  return recipientName.value && recipientAddress.value && 
  postalCode.value && phoneNumber.value && 
  email.value && deliveryMethod.value;
});
</script>


<template>
  <div class="fixed inset-0 flex items-center justify-center z-30">
    <div @click="closeDrawer" class="fixed inset-0 bg-black opacity-70">  
    </div>
    <div class="bg-white w-4/5 max-w-[50rem] max-h-[90vh] rounded-lg shadow-xl z-40 p-8 overflow-y-auto relative">
      <DrawerHead />

      <div v-if="!totalPrice || orderId" class="flex flex-col items-center justify-center h-full">
        <InfoBlock
          v-if="!totalPrice && !orderId"
          Title="Корзина пустая"
          Description="Добавьте хотя бы одну пару кроссовок, чтобы сделать заказ."
          Image="/package-icon.png"
        />
        <InfoBlock
          v-if="orderId"
          Title="Заказ оформлен!"
          :Description="`Ваш заказ #${orderId} скоро будет передан курьерской доставке`"
          Image="/order-success-icon.png"
        />
      </div>

      <div v-else>
        <CartItemList />
        <div class="flex gap-2 mt-4">
            <span>Итого:</span>
            <div class="flex-1 border-b border-dashed"></div>
            <b>{{ totalPrice }} руб.</b>
          </div>
        
        <div v-if="errorMessage" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mt-4">
          {{ errorMessage }}
        </div>
        
        <div class="flex flex-col gap-4 mt-7">
          <div class="flex flex-col">
            <label for="recipientName" class="font-medium mt-2">ФИО получателя</label>
            <input id="recipientName" v-model="recipientName" placeholder="ФИО" class="border rounded p-3" />
          </div>
          <div class="flex flex-col">
            <label for="recipientAddress" class="font-medium mt-2">Адрес получателя</label>
            <input id="recipientAddress" v-model="recipientAddress" placeholder="Страна, Город, Улица, Дом, Квартира" class="border rounded p-3" />
          </div>
          <div class="flex flex-col">
            <label for="postalCode" class="font-medium mt-2">Почтовый индекс</label>
            <input id="postalCode" v-model="postalCode" placeholder="100000" class="border rounded p-3" />
          </div>
          <div class="flex flex-col">
            <label for="phoneNumber" class="font-medium mt-2">Телефон получателя</label>
            <input id="phoneNumber" v-model="phoneNumber" placeholder="+7 (999) 999-99-99" class="border rounded p-3" />
          </div>
          <div class="flex flex-col">
            <label for="email" class="font-medium mt-2">Email получателя</label>
            <input id="email" v-model="email" placeholder="Email" class="border rounded p-3" />
          </div>
          <div class="flex flex-col">
            <label for="promoCode" class="font-medium mt-2">Промокод</label>
            <input id="promoCode" v-model="promoCode" placeholder="Введите промокод" class="border rounded p-3" />
          </div>
          <div class="flex items-center">
            <input type="radio" id="deliveryCDEK" value="CDEK" v-model="deliveryMethod" class="mr-3" />
            <label for="deliveryCDEK" class="font-medium">CDEK (Оплата при получении)</label>
          </div>
          <div class="flex items-center">
            <input type="radio" id="deliveryRussianPost" value="RussianPost" v-model="deliveryMethod" class="mr-3" />
            <label for="deliveryRussianPost" class="font-medium">Почта России (Оплата при получении)</label>
          </div>

          <div class="flex gap-2 mt-4">
            <span>Итого:</span>
            <div class="flex-1 border-b border-dashed"></div>
            <b>{{ totalPrice }} руб.</b>
          </div>
          <button
            :disabled="ButtonDisabled"
            @click="createOrder"
            class="mt-4 transition bg-sky-500 w-full rounded-xl py-3 text-white disabled:bg-slate-400 hover:bg-sky-600 active:bg-sky-700 cursor-pointer"
          >
            {{ isCreating ? 'Оформление...' : 'Оформить заказ' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

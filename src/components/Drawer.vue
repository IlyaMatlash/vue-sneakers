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

const createOrder = async () => {
  //formSubmitted.value = true;
  if (!isFormValid.value || cartIsEmpty.value) return;

  try {
    isCreating.value = true
    const { data } = await axios.post(`https://6e08a32863a3f798.mokky.dev/orders`, {
      items: cart.value,
      totalPrice: props.totalPrice.value,
      recipientName: recipientName.value,
      recipientAddress: recipientAddress.value,
      postalCode: postalCode.value,
      phoneNumber: phoneNumber.value,
      email: email.value,
      promoCode: promoCode.value,
      deliveryMethod: deliveryMethod.value
    })

    cart.value = []

    orderId.value = data.id;
  } catch (error) {
    console.log(error)
  } finally {
    isCreating.value = false
  }
}

const cartIsEmpty = computed(() => cart.value.length === 0)
const ButtonDisabled = computed(() => isCreating.value || cartIsEmpty.value || !isFormValid.value)

// Validation logic
const isFormValid = computed(() => {
  return recipientName.value && recipientAddress.value && postalCode.value && phoneNumber.value && email.value && deliveryMethod.value;
})
</script>


<template>
  <div class="fixed inset-0 flex items-center justify-center z-30">
    <div @click="closeDrawer" class="fixed inset-0 bg-black opacity-70">  
    </div>
    <div class="bg-white w-4/5 w-full max-w-[50rem] max-h-[90vh] rounded-lg shadow-xl z-40 p-8 overflow-y-auto relative">
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
            <input type="radio" id="deliveryCDEK" v-model="deliveryMethod" class="mr-3" />
            <label for="deliveryCDEK" class="font-medium mt-2">CDEK</label>
          </div>
          <div class="flex items-center">
            <input type="radio" id="deliveryRussianPost" v-model="deliveryMethod" class="mr-3" />
            <label for="deliveryRussianPost" class="font-medium mt-2">Почта России</label>
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
            Оформить заказ
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

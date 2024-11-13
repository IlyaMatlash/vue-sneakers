<script setup>
const props = defineProps({
  Productid: Number,
  Name: String,
  Description: String,
  Image: String,
  Price: Number,
  isFavorite: Boolean,
  isAdded: Boolean,
  onClickAdd: Function,
  onClickFavorite: Function
})

const visibleFavoriteButton = Boolean(props.onClickAdd)
</script>

<template>
  <div
    class="relative bg-white border-slate-100 rounded-3xl p-8 cursor-pointer transition hover:-translate-y-2 hover:shadow-xl"
  >
    <img
      v-if="visibleFavoriteButton"
      @click="onClickFavorite"
      :src="!isFavorite ? '/like-1.svg' : '/like-2.svg'"
      class="absolute top-8 left-8 mb-5"
      alt="Like"
    />
    <img class="w-full" :src="Image" alt="Sneaker" />
    <p class="mt-2">{{ Name }}</p>

    <div class="flex justify-between mt-5">
      <div class="flex flex-col">
        <span class="text-slate-400">Цена:</span>
        <b>{{ Price }} руб.</b>
      </div>
      <img v-if="onClickFavorite" @click="onClickAdd" :src="!isAdded ? '/plus.svg' : '/checked.svg'" alt="Plus" />
    </div>
  </div>
</template>

<script setup>
import { getFirstImage } from '../utils/imageUtils';

const props = defineProps({
  Productid: Number,
  Name: String,
  Description: String,
  Images: {
    type: [String, Array],
    required: true,
    default: () => ['/sneakers/default-image.jpg'],
    validator(value) {
      if (typeof value === 'string') return true;
      return Array.isArray(value) && value.length > 0;
    }
  },
  Price: Number,
  isFavorite: Boolean,
  isAdded: Boolean,
  onClickAdd: Function,
  onClickFavorite: Function
})

const emit = defineEmits(['openCardModal'])

const visibleFavoriteButton = Boolean(props.onClickAdd)
</script>

<template>
  <button 
    @click="$emit('click')"
    class="relative bg-white border-slate-100 rounded-3xl p-8 cursor-pointer transition hover:-translate-y-2 hover:shadow-xl">
    <img
      v-if="visibleFavoriteButton"
      @click.stop="onClickFavorite"
      :src="!isFavorite ? '/like-1.svg' : '/like-2.svg'"
      class="absolute top-8 left-8 mb-5"
      alt="Like"
    />
    <img class="w-full" :src="getFirstImage(Images)" :alt="Name" />
    <p class="mt-2 text-left">{{ Name }}</p>

    <div class="flex justify-between mt-5">
      <div class="flex flex-col">
        <span class="text-slate-400 text-left">Цена:</span>
        <b>{{ Price }} руб.</b>
      </div>
      <img v-if="onClickFavorite" @click.stop="onClickAdd" :src="!isAdded ? '/plus.svg' : '/checked.svg'" alt="Plus" />
    </div>
  </button>
</template>
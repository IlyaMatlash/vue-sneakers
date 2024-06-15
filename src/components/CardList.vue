<script setup>
import { inject } from 'vue'
import Card from './Card.vue'

defineProps({
  items: Array
})

const onClickAdd = () => {
  alert('Добавлено в корзину')
}

const addToFavorite = inject('addToFavorite')

</script>

<template>
  <div class="grid grid-cols-4 gap-5">
    <Card
      v-for="item in items"
      :key="item.id"
      :id="item.id"
      :title="item.title"
      :image-url="item.imageUrl"
      :price="item.price"
      @click-add="onClickAdd"
      :on-click-favorite="() => addToFavorite(item)"
      :is-added="false"
      :is-favorite="item.isFavorite"
    />
  </div>
</template>

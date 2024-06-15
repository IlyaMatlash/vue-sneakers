<script setup>
import Card from './Card.vue'

defineProps({
  items: Array
})

const emit = defineEmits(['addToFavorite'])
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
      :on-click-favorite="() => emit('addToFavorite', item)"
      :is-added="false"
      :is-favorite="item.isFavorite"
    />
  </div>
</template>

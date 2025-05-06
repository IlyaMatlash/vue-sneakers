<script setup>
import { ref } from 'vue';
import Card from './Card.vue'
import CardModal from './CardModal.vue';

const props = defineProps({
  items: Array,
  required: true
})

const selectedItem = ref(null);
const isModalOpen = ref(false);

const openCardModal = (item) => {
  selectedItem.value = item;
  isModalOpen.value = true;
}

const closeCardModal = () => {
  isModalOpen.value = false;
  selectedItem.value = null;
}

const emit = defineEmits(['addToFavorite', 'addToCart'])
</script>

<template>
  <div v-if="items.length > 0" class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4" v-auto-animate>
    <Card
      v-for="item in items"
      :key="item.id"
      :id="item.id"
      :Name="item.Name"
      :description="item.description"
      :Images="item.Images"
      :Price="item.Price"
      :on-click-favorite="isFavorites ? null : () => emit('addToFavorite', item)"
      :on-click-add="isFavorites ? null : () => emit('addToCart', item)"
      :is-favorite="item.isFavorite"
      :is-added="item.isAdded"
      @click="openCardModal(item)"
    />
    <CardModal
      v-if="isModalOpen"
      :item="selectedItem"
      :is-favorite="selectedItem?.isFavorite"
      :is-added="selectedItem?.isAdded"
      @close="closeCardModal"
      @add-to-favorite="$emit('addToFavorite', $event)"
      @add-to-cart="$emit('addToCart', $event)"
    />
  </div>
</template>

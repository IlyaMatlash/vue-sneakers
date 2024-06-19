<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import CardList from '../components/CardList.vue'
import InfoBlock from '@/components/InfoBlock.vue';

const favorites = ref([])
onMounted(async () => {
  try {
    const { data } = await axios.get(
      'https://6e08a32863a3f798.mokky.dev/favorites?_relations=items'
    )
    favorites.value = data.map((obj) => obj.item)
  } catch (error) {
    console.log(error)
  }
})
</script>

<template>
  <h2 class="text-3xl font-bold mb-8">Мои Закладки</h2>

  <div>
    <InfoBlock
    v-if="!is-favorites"
    title="Закладок нет :("
    description="Вы ничего не добавляли в закладки"
    image-url="/emoji-1.png"
  />
  <CardList :items="favorites" is-favorites />
  </div>
  
</template>

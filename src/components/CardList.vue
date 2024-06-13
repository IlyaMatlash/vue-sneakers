<script setup>
import Card from './Card.vue'

const onClickAdd = () => {
    alert('Добавлено в корзину')
}
</script>

<template>
  <div class="grid grid-cols-4 gap-5">
    <Card 
    title="Мужские Кроссовки Nike Blazer Mid Suede" 
    image-url="/sneakers/sneakers-1.jpg" 
    :price="5000"
    @click-add="onClickAdd" 
    :is-added="false" 
    :is-favorite="true"
    />
  </div>
</template>

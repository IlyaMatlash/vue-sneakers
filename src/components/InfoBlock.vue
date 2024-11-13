<script setup>
defineProps({
  Name: String,
  Description: String,
  Image: String
})
</script>

<template>
  <div class="flex flex-col items-center text-center w-72 mx-auto">
    <img height="80" width="80" :src="Image" alt="Info image" />
    <h2 class="mt-4 text-2xl font-medium">{{ Name }}</h2>
    <p class="mt-2 text-gray-400">{{ Description }}</p>
  </div>
</template>

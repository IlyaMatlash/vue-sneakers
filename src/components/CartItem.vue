<script setup>
const emit = defineEmits(['onClickRemove'])

defineProps({
  Productid: Number,
  Name: String,
  Description: String,
  Image: String,
  Price: Number
})
</script>

<template>
  <div class="flex border border-slate-200 p-4 rounded-xl gap-4">
    <img class="w-16 h-16" :src="Image" :alt="Name" />

    <div class="flex flex-col flex-1">
      <p>{{ Name }}</p>

      <div class="flex justify-between mt-2">
        <b class="flex-1">{{ Price }}</b>
        <img
          @click="emit('onClickRemove')"
          class="opacity-40 hover:opacity-100 cursor-pointer transition"
          src="/close.svg"
          alt="Close"
        />
      </div>
    </div>
  </div>
</template>

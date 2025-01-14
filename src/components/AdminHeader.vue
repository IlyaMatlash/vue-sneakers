<template>
    <div class="flex gap-3 mb-4">
        <router-link to="/admin">
            <button
            type="button" 
            class="rounded-lg px-4 py-2 bg-pink-200 hower:bg-pink-300 transition duration-300 font-medium">Товары
            </button>
        </router-link>
        <router-link to="/users">
            <button 
            type="button" 
            class="rounded-lg px-4 py-2 bg-pink-200 hower:bg-pink-300 transition duration-300 font-medium">Пользователи
            </button>
        </router-link>
    </div>
</template>
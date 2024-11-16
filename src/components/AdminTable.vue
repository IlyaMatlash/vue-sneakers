<template>
    <table class="border-separate border-spacing-0 w-full bg-white shadow-lg">
        <thead>
            <tr class="bg-slate-200 text-slate-800">
                <th class="border border-slate-300 p-2"
                v-for="header in headerItems" :key="header"
                >
                {{ header }}   
                </th>
            </tr>
        </thead>
        <tbody>
                <tr v-for="(row, index) in rows" :key="index" class="hover:bg-slate-100 transition duration-300">
                <td v-for="(column, colIndex) in Object.values(row)" :key="colIndex"
                class="border border-slate-300 p-2 text-left">
                    {{ column }}
                </td>
            </tr>
        </tbody>
    </table>
</template>

<script setup>
import { defineProps } from 'vue';

    const props = defineProps ({
        rows: Array,
        headerItems: Array,
        required: true
    })
</script>

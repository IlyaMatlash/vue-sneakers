<template>
  <main>
    <AdminTable :rows="rows" :headerItems="headerItems" />
  </main>
</template>

<script setup>
import AdminTable from "@/components/AdminTable.vue";
import { onMounted, reactive} from "vue";
import axios from "axios";

const rows = reactive([]);
const headerItems = reactive([]);

onMounted(async () => {
  try {
    const response = await axios.get(`http://localhost:5072/api/product`);
    rows.push(...response.data);
    if (rows.length > 0) {
      headerItems.push(...Object.keys(rows[0]));
    }
  } catch (error) {
    console.error(error);
  }
});
</script>


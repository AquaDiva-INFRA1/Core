<script>
	import Fa from 'svelte-fa';
	import { faFileAlt } from '@fortawesome/free-solid-svg-icons';

	export let id;
	export let index;
	export let name;
	export let description;
	export let entityType;
	export let metadataStructure;
	export let allowedFileTypes = [];
	export let inUse = false;
</script>

<!-- svelte-ignore a11y-click-events-have-key-events -->

<div 
	class="flex gap-5 border-b-violet-700 p-5 shadow-md ring-1 ring-gray-900/5
hover:bg-primary-100 space-x-100 active:bg-primary-300"
	on:click
>
	<div class="flex-none text-left">
		<Fa size="4x" icon={faFileAlt} />
	</div>
	<div class="text-left grow h-full">
		<h3 class="h3">{name}</h3>
		<span class="text-sm">{description}</span>
	</div>
	<div class="flex-none text-right">
		<div class="flex gap-2">
			{#if inUse}
				<span class="badge variant-filled-error">in use</span>
			{/if}
			<span class="badge variant-filled-surface">{metadataStructure.text}</span>
			<span class="badge variant-filled-secondary">{entityType.text}</span>
		</div>
		<div class="h-content">
			<b>
				{allowedFileTypes.join(', ')}
			</b>
		</div>
	</div>
</div>

class ChipScroller {
    static scrollToChip(container: HTMLElement, chipId: string): void {
        if (!container) {
            return;
        }

        const chip = document.getElementById(chipId);
        if (!chip) {
            return;
        }
       
        const scrollLeft = chip.offsetLeft - container.offsetLeft;
        
        container.scrollTo({
            left: scrollLeft,
            behavior: 'smooth'
        });
    }
}

export default ChipScroller;

class IntersectionObserverManager {
    private static observer: IntersectionObserver | null = null;

    /**
     * Observes an element and triggers callback when it becomes visible
     * @param element - The DOM element to observe
     * @param dotNetRef - The .NET object reference for callback
     */
    public static observe(element: Element, dotNetRef: any): void {
        this.dispose();
        
        if (!element) {
            return;
        }

        this.observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    dotNetRef.invokeMethodAsync('OnLoadTriggerVisible');
                }
            });
        }, {
            root: null,
            rootMargin: '100px', 
            threshold: 0
        });

        this.observer.observe(element);
    }

    /**
     * Disposes the current IntersectionObserver instance
     */
    public static dispose(): void {
        if (this.observer) {
            this.observer.disconnect();
            this.observer = null;
        }
    }
}

export default IntersectionObserverManager;

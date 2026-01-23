import FileDownloader from './modules/file-downloader';
import IntersectionObserverManager from './modules/intersection-observer';
import ChipScroller from './modules/chip-scroller';

declare global {
    interface Window {
        FileDownloader: typeof FileDownloader;
        IntersectionObserverManager: typeof IntersectionObserverManager;
        scrollToChip: typeof ChipScroller.scrollToChip;
    }
}

window.FileDownloader = FileDownloader;
window.IntersectionObserverManager = IntersectionObserverManager;
window.scrollToChip = ChipScroller.scrollToChip;
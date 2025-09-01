import FileDownloader from './modules/file-downloader';
import IntersectionObserverManager from './modules/intersection-observer';

declare global {
    interface Window {
        FileDownloader: typeof FileDownloader;
        IntersectionObserverManager: typeof IntersectionObserverManager;
    }
}

window.FileDownloader = FileDownloader;
window.IntersectionObserverManager = IntersectionObserverManager;
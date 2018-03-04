import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'long-running',
    templateUrl: './longrunning.component.html'
})
export class LongRunningComponent {
    public result: LongRunningTaskResult;

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/LongRunning/Start')
            .subscribe(result => {
                this.result = result.json() as LongRunningTaskResult;
        }, error => console.error(error));
    }
}

interface LongRunningTaskResult {
    result: string;
}

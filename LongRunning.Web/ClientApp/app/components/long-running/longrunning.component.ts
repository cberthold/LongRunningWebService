import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';
import 'isomorphic-fetch';

@Component({
    selector: 'long-running',
    templateUrl: './longrunning.component.html'
})
export class LongRunningComponent {
    public result: LongRunningTaskResult;
    public loading: boolean;

    constructor(http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.loading = false;
    }

    public startLongRunning = () => {
        this.loading = true;
        this.fetchLongRunning(this.baseUrl + 'api/LongRunning/Start')
            .then(async data => {
                this.loading = false;
                this.result = await data.json() as LongRunningTaskResult;
            }).catch(error => {
                this.loading = false;
                console.error(error);
            });
    }

    private fetchLongRunning = (input: RequestInfo, init?: RequestInit | undefined): Promise<Response> => {
        return new Promise<Response>(async (resolve, reject) => {
            try {
                let response = await fetch(input, init);

                if (response.status != 202) {
                    resolve(response);
                    return;
                }

                const resolveLoop = async () => {

                    let checkUrl = response.headers.get("Location") as string;
                    if (checkUrl.endsWith('/')) {
                        checkUrl = checkUrl.substring(0, checkUrl.length - 1);
                    }
                    response = await fetch(this.baseUrl + checkUrl, init);

                    if (response.status != 202) {
                        resolve(response);
                        return;
                    }

                    setTimeout(resolveLoop, 1000);
                }
                setTimeout(resolveLoop, 1000);


            }
            catch (err) {
                reject(err);
            }
        });
    }
}

interface LongRunningTaskResult {
    result: string;
}

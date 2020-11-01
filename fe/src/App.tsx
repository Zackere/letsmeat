import * as React from 'react';
import { BrowserRouter, Switch } from 'react-router-dom';
import { Provider } from 'react-redux';
import { ThemedContainer } from './modules/theme/components';
import store from './store';

function App(): React.ReactElement {
  return (
    <Provider store={store}>
      <ThemedContainer>
        <BrowserRouter>
          <Switch>
            {/** Code */}
          </Switch>
        </BrowserRouter>
      </ThemedContainer>
    </Provider>
  );
}

export default App;

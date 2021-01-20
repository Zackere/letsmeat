process.env['NODE_TLS_REJECT_UNAUTHORIZED'] = 0;
const axios = require('axios');
async function main(api) {
  const usersSeed = [{ id: '123', picture_url: 'idk', email: 'example@gmail.com', name: 'John Snow 1', token: 't1' }];
  const users = [];
  for (const seed of usersSeed) {
    try {
      users.push((await axios.post(`${api}/login/fake`, seed)).data);
    } catch (e) {
      if (e.response.status !== 409) throw e;
      users.push(e.response.data);
    }
  }
  for (let i = 0; i < 10; ++i) await axios.post(`${api}/groups/create?token=${users[0].token}`, { name: 'Cool bois' });
  await axios.get(`${api}/users/info?token=${users[0].token}`);
}
main('https://localhost:49153').catch(console.log);

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
  const groupCreate = await axios.post(`${api}/groups/create?token=${users[0].token}`, { name: 'Cool bois' });
  await Promise.all(
    new Array(100).fill(0).map(_ =>
      axios.post(`${api}/locations/create_custom?token=${users[0].token}`, {
        group_id: groupCreate.data.id,
        address: 'aaaaaaaaaaaaaa',
        name: 'aaaaaaaaaaaaaa',
      }),
    ),
  );
  await Promise.all(
    new Array(100)
      .fill(0)
      .map(_ =>
        axios.get(
          `${api}/locations/search?token=${users[0].token}&group_id=${groupCreate.data.id}&query_string=aaa&sessiontoken=aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa`,
        ),
      ),
  );
}
main('https://localhost:49153').catch(console.log);

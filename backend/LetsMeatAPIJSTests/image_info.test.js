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
  const eventCreate = await axios.post(`${api}/events/create?token=${users[0].token}`, {
    group_id: groupCreate.data.id,
    name: 'Cool bois event',
    deadline: new Date('December 17, 2030'),
  });
  const ids = await Promise.all(
    new Array(100)
      .fill(0)
      .map(_ =>
        axios.post(
          `${api}/images/mock_upload?token=${users[0].token}&event_id=${eventCreate.data.id}&name=imageeeeee.png`,
        ),
      ),
  );
  await axios.post(`${api}/images/info?token=${users[0].token}`, { image_ids: ids.map(i => i.data) });
}
main('https://localhost:49153').catch(console.log);

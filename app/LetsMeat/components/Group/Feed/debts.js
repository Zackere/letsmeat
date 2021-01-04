import { MaterialCommunityIcons as Icon } from '@expo/vector-icons';
import React, { useContext, useEffect, useState } from 'react';
import {
  Image, StyleSheet, View
} from 'react-native';
import ImagePicker from 'react-native-image-crop-picker';
import {
  ActivityIndicator, Button, Caption, Card, Paragraph
} from 'react-native-paper';
import { formatAmount } from '../../../helpers/money';
import {
  getImagesInfo,
  uploadImage, deleteImage, deleteImageDebt, createImageDebt, addDebt, getPendingDebts, getUsersInfo, getEventDebts
} from '../../Requests';
import { store } from '../../Store';
import { UserPicker } from '../../User';

// const Debt = ({
//   debt, owner = false, assignedTo, onAssign, onClaim, onEdit, navigation, imageId
// }) => {
//   const { state } = useContext(store);
//   const [visible, setVisible] = useState(true);
//   const [userPickerOpen, setUserPickerOpen] = useState(false);
//   const [user, setUser] = useState(assignedTo || null);
//   if (debt.amount === 8100) { console.log(assignedTo); console.log(debt); }

//   return (
//     visible ? (
//       <>
//         <Card style={{ margin: 5 }}>
//           <Card.Title title={debt.amount / 100} />
//           <Card.Content>
//             <Caption>
//               {debt.satisfied
//                 ? (
//                   user && `This debt has been accepted by
//                 ${user.name}`
//                 )
//                 : (user
//                   ? (user && `This debt has been assigned to ${user.name}`)
//                   : 'No one has claimed this debt yet')}
//             </Caption>
//             <Paragraph>
//               {debt.description}
//             </Paragraph>
//           </Card.Content>
//           <Card.Actions style={{ justifyContent: 'space-evenly' }}>
//             {!owner && (
//             <Button
//               onPress={onClaim}
//             >
//               Claim
//             </Button>
//             )}
//             {owner && (
//             <>
//               <Button
//                 onPress={() => setUserPickerOpen(true)}
//               >
//                 Assign
//               </Button>
//               <Button
//                 onPress={() => {
//                   navigation.navigate('AddDebt', { imageId, debt });
//                 }}
//               >
//                 Edit
//               </Button>
//               <Button
//                 color="red"
//                 onPress={() => {
//                   deleteImageDebt({ state }, debt.id).then(setVisible(false));
//                 }}
//               >
//                 Delete
//               </Button>
//             </>
//             )}
//           </Card.Actions>
//         </Card>
//         <UserPicker
//           userIds={state.group.users.map((u) => u.id).filter((id) => id !== state.user.id)}
//           dialogVisible={userPickerOpen}
//           onDismiss={() => setUserPickerOpen(false)}
//           setUser={(newUser) => {
//             addDebt({ state }, state.group.id, state.event.id, newUser.id, state.user.id, null, null, debt.id, 0)
//               .then(() => setUser(newUser));
//           }}
//         />
//       </>
//     ) : null);
// };

const Debt = ({
  image, debt, users, navigation, setDebts, setImages
}) => {
  const { state, dispatch } = useContext(store);
  const [visible, setVisible] = useState(true);
  const [userPickerOpen, setUserPickerOpen] = useState(false);
  let assignedUser;
  if (debt.pending_debt) {
    assignedUser = users.find((u) => u.id === debt.pending_debt.to_id);
  }

  const owner = image.uploaded_by === state.user.id;

  return (
    visible ? (
      <>
        <Card style={{ margin: 5 }}>
          <Card.Title
            title={formatAmount(debt.amount)}
            subtitle={
            debt.satisfied
              ? (
                assignedUser ? `This debt has been accepted by ${assignedUser.name}`
                  : 'Someone agreed to pay for this'
              )
              : (assignedUser
                ? (assignedUser && `This debt has been assigned to ${assignedUser.name}`)
                : 'No one has claimed this debt yet')
          }
          />
          <Card.Content>
            <Paragraph>
              {debt.description}
            </Paragraph>
          </Card.Content>
          <Card.Actions style={{ justifyContent: 'space-evenly' }}>
            {owner ? (
              <>
                <Button
                  onPress={() => setUserPickerOpen(true)}
                >
                  Assign
                </Button>
                <Button
                  onPress={() => {
                    navigation.navigate('AddDebt', { imageId: image.image_id, debt });
                  }}
                >
                  Edit
                </Button>
                <Button
                  color="red"
                  onPress={() => {
                    deleteImageDebt({ state }, debt.id).then(setVisible(false));
                  }}
                >
                  Delete
                </Button>
              </>
            ) : (
              <Button
                onPress={() => { dispatch({ type: 'SET_EVENT', payload: state.event }); }}
              >
                Claim
              </Button>
            )}
          </Card.Actions>
        </Card>
        {/* <UserPicker
          userIds={state.group.users.map((u) => u.id).filter((id) => id !== state.user.id)}
          dialogVisible={userPickerOpen}
          onDismiss={() => setUserPickerOpen(false)}
          setUser={(newUser) => {
            addDebt({ state }, state.group.id, state.event.id, newUser.id, state.user.id, null, null, debt.id, 0)
              .then(() => setUser(newUser));
          }}
        /> */}
      </>
    ) : null);
};

// const DebtImage = ({ id, navigation }) => {
//   const { state, dispatch } = useContext(store);
//   const [imageData, setImageData] = useState(null);
//   const [imageSize, setImageSize] = useState({ width: 0, height: 0 });
//   const [users, setUsers] = useState([]);
//   const [debts, setDebts] = useState([]);

//   useEffect(() => {
//     getImagesInfo({ state }, id).then((info) => {
//       getPendingDebts({ state }, info[0].debts_from_image.map((d) => d.id))
//         .then((debts) => {
//           setDebts(debts);
//           return debts.map((d) => d.from_id);
//         })
//         .then((fromIds) => getUsersInfo({ state }, fromIds))
//         .then((users) => {
//           // console.log('Users');
//           // console.log(users);
//           setUsers(users);
//         });
//       Image.getSize(info[0].image_url, (width, height) => {
//         setImageSize({ width, height });
//         setImageData(info[0]);
//       });
//     });
//   }, [id, state]);

//   return (
//     imageData
//       ? (
//         <Card style={{ margin: 5 }}>
//           <Card.Content>
//             <Image
//               style={{ width: '100%', height: undefined, aspectRatio: imageSize.width / imageSize.height || 0 }}
//               source={{ uri: imageData.image_url }}
//             />
//           </Card.Content>
//           <Card.Content>
//             {(debts || [])
//               .map((d) => (
//                 <Debt
//                   debt={d}
//                   key={d.id}
//                   owner={state.user.id === imageData.uploaded_by}
//                   navigation={navigation}
//                   imageId={id}
//                   assignedTo={users.find((u) => d.from_id === u.id)}
//                 />
//               ))}
//           </Card.Content>
//           <Card.Actions style={{ justifyContent: 'space-evenly' }}>
//             <Button onPress={
//               () => {
//                 navigation.navigate('AddDebt', { eventId: state.event.id, imageId: id });
//               }
//             }
//             >
//               <Icon name="plus" size={25} />
//             </Button>
//             <Button onPress={
//               () => {
//                 deleteImage({ state }, id).then(() => {
//                   dispatch({ type: 'REMOVE_IMAGE', imageId: id });
//                 });
//               }
//             }
//             >
//               <Icon name="delete" size={25} />
//             </Button>
//           </Card.Actions>
//         </Card>
//       )
//       : <ActivityIndicator />
//   );
// };

const DebtImage = ({
  image, users, debts, navigation, setDebts, setImages
}) => {
  const { state, dispatch } = useContext(store);
  const [imageSize, setImageSize] = useState({ width: 0, height: 0 });

  useEffect(() => {
    Image.getSize(image.image_url, (width, height) => {
      setImageSize({ width, height });
    });
  }, [image.image_url]);

  return (
    <Card style={{ margin: 5 }}>
      <Card.Content>
        <Image
          style={{ width: '100%', height: undefined, aspectRatio: imageSize.width / imageSize.height || 0 }}
          source={{ uri: image.image_url }}
        />
      </Card.Content>
      <Card.Content>
        {(debts.filter((d) => d.image_id === image.image_id))
          .map((d) => (
            <Debt
              key={d.id}
              debt={d}
              image={image}
              setImages={setImages}
              users={users}
              navigation={navigation}
              setDebts={setDebts}
            />
          ))}
      </Card.Content>
      <Card.Actions style={{ justifyContent: 'space-evenly' }}>
        <Button onPress={
                () => {
                  navigation.navigate('AddDebt', { eventId: state.event.id, imageId: image.image_id });
                }
              }
        >
          <Icon name="plus" size={25} />
        </Button>
        <Button onPress={
                () => {
                  deleteImage({ state }, id).then(() => {
                    dispatch({ type: 'REMOVE_IMAGE', imageId: id });
                  });
                }
              }
        >
          <Icon name="delete" size={25} />
        </Button>
      </Card.Actions>
    </Card>
  );
};

const Images = ({ images, navigation }) => (images
  ? images.map((id) => (
    <DebtImage
      navigation={navigation}
      key={id}
      id={id}
    />
  ))
  : <></>);

const Debts = ({ navigation }) => {
  const { state, dispatch } = useContext(store);
  const [loading, setLoading] = useState(true);
  const [users, setUsers] = useState([]);
  const [debts, setDebts] = useState([]);
  const [images, setImages] = useState([]);

  useEffect(() => {
    (async () => {
      const [imagesInfo, debtsInfo] = await Promise.all([
        getImagesInfo({ state }, state.event.images), getEventDebts({ state }, state.event.id)]);
      const usersInfo = await getUsersInfo({ state }, [
        ...imagesInfo.map((info) => info.uploaded_by),
        ...debtsInfo.filter((debt) => debt.pending_debt).map((debt) => debt.pending_debt.from_id),
        ...debtsInfo.filter((debt) => debt.pending_debt).map((debt) => debt.pending_debt.to_id),
      ]);
      setUsers(usersInfo);
      setDebts(debtsInfo);
      setImages(imagesInfo);
    })();
  }, [state.event.images, state.user.token]);

  return (
    <Card style={styles.section} elevation={0}>
      <Card.Title title="Debts" />
      {/* <Images
        navigation={navigation}
        images={images}
      /> */}
      {/* {images && images.map((i) => <Paragraph key={i.image_id}>{i.image_id}</Paragraph>)} */}
      {(images) && images.map((i) => (
        <DebtImage
          key={i.image_id}
          image={i}
          setImages={setImages}
          users={users}
          debts={debts}
          setDebts={setDebts}
          navigation={navigation}
        />
      ))}
      <View style={{ flexDirection: 'row', justifyContent: 'space-evenly' }}>
        <Button
          style={styles.addButton}
          onPress={
          () => {
            ImagePicker.openPicker({
              cropping: true
            })
              .then((i) => uploadImage({ state }, state.event.id, i))
              .then((data) => {
                dispatch({ type: 'ADD_IMAGE_TO_EVENT', imageId: data.image_id });
              })
              .catch((e) => {
                if (e.code === 'E_PICKER_CANCELLED') return;
                throw e;
              });
          }
        }
        >
          <Icon name="image-plus" size={25} />
        </Button>
      </View>
    </Card>
  );
};

const styles = StyleSheet.create({
  eventTitle: {
    fontSize: 30,
    marginHorizontal: 20,
    marginTop: 20
  },
  container: {
    width: '100%',
    height: '100%'
  },
  section: {
    margin: 10
  },
  card: {
    margin: 25
  },
  addButton: {
    marginBottom: 10
  },
  timeCard: {
    margin: 5,
    justifyContent: 'center'
  }
});

export default Debts;
